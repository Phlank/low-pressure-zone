using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using LowPressureZone.Data;
using LowPressureZone.Domain.BroadcastAggregate;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace LowPressureZone.Api.Commands.Broadcasts.SyncBroadcasts;

[RegisterService<SyncBroadcastsHandler>(LifeTime.Scoped)]
public class SyncBroadcastsHandler(
    DataContext dataContext,
    IAzuraCastClient azuraCastClient,
    ILogger<SyncBroadcastsHandler> logger) : ICommandHandler<SyncBroadcastsCommand, List<Broadcast>>
{
    public async Task<List<Broadcast>> ExecuteAsync(SyncBroadcastsCommand command, CancellationToken ct)
    {
        var domainBroadcasts = await dataContext.Broadcasts.ToDictionaryAsync(broadcast => broadcast.AzuraCastBroadcastId, ct);
        var remoteBroadcastsResult = await azuraCastClient.GetBroadcastsAsync();
        if (remoteBroadcastsResult.IsError)
        {
            logger.LogError("Error while fetching remote broadcasts: {ErrorMessage}",
                            remoteBroadcastsResult.Error.ReasonPhrase);

            return [ ..domainBroadcasts.Values];
        }
        logger.LogInformation("Fetched {Count} remote broadcasts from AzuraCast", remoteBroadcastsResult.Value.Count);

        var remoteBroadcasts = remoteBroadcastsResult.Value.ToDictionary(broadcast => broadcast.Id);

        var remoteIds = remoteBroadcasts.Keys;
        var domainIds = domainBroadcasts.Keys;
        var idsToDelete = domainIds.Except(remoteIds);
        var idsToAdd = remoteIds.Except(domainIds);
        var idsToUpdate = domainIds.Intersect(remoteIds);
        
        foreach (var id in idsToDelete)
        {
            DeleteBroadcastFromDomain(domainBroadcasts[id]);
        }

        foreach (var id in idsToAdd)
        {
            AddBroadcastToDomain(remoteBroadcasts[id]);
        }
        
        foreach (var id in idsToUpdate)
        {
            UpdateBroadcastInDomain(domainBroadcasts[id], remoteBroadcasts[id]);
        }
        
        await dataContext.SaveChangesAsync(ct);
        return await dataContext.Broadcasts.ToListAsync(ct);
    }
    
    private void DeleteBroadcastFromDomain(Broadcast domainBroadcast)
        => dataContext.Broadcasts.Remove(domainBroadcast);

    private void AddBroadcastToDomain(StationStreamerBroadcast remoteBroadcast)
    {
        remoteBroadcast.Streamer.ShouldNotBeNull();
        var broadcastResult = Broadcast.Create(remoteBroadcast.Id,
                                               remoteBroadcast.Streamer.Id,
                                               remoteBroadcast.Streamer.DisplayName,
                                               remoteBroadcast.Recording?.DownloadUrl is not null,
                                               remoteBroadcast.TimestampStart,
                                               remoteBroadcast.TimestampEnd);
        if (broadcastResult.IsError)
        {
            logger.LogWarning("Failed to add new broadcast to domain: {ErrorMessages}",
                              string.Join(",", broadcastResult.Errors.Select(e => e.Message)));
            return;
        }

        dataContext.Add(broadcastResult.Value);
    }

    private void UpdateBroadcastInDomain(Broadcast domainBroadcast,
                                         StationStreamerBroadcast remoteBroadcast)
    {
        remoteBroadcast.Streamer.ShouldNotBeNull();
        List<DomainResult<NoValue>> results = [];

        if (!domainBroadcast.HasFile && remoteBroadcast.Recording?.DownloadUrl is not null)
        {
            results.Add(domainBroadcast.SetHasFile(true));
        }
        else if (domainBroadcast.HasFile && remoteBroadcast.Recording?.DownloadUrl is null)
        {
            results.Add(domainBroadcast.SetHasFile(false));
        }

        if (!domainBroadcast.Time.EndsAt.HasValue && remoteBroadcast.TimestampEnd.HasValue)
        {
            results.Add(domainBroadcast.SetEnd(remoteBroadcast.TimestampEnd.Value));
        }

        if (domainBroadcast.AzuraCastStreamerDisplayName != remoteBroadcast.Streamer.DisplayName)
        {
            domainBroadcast.SetDisplayName(remoteBroadcast.Streamer.DisplayName);
        }

        var composedResult = DomainResult.Compose(results);
        if (composedResult.IsError)
        {
            logger.LogWarning("Domain errors while updating broadcast: {ErrorMessages}",
                              string.Join(",", composedResult.Errors.Select(e => e.Message)));
        }
    }
}