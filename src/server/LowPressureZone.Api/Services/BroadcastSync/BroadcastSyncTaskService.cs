using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using LowPressureZone.Data;
using LowPressureZone.Domain.BroadcastAggregate;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace LowPressureZone.Api.Services.BroadcastSync;

public class BroadcastSyncTaskService(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<BroadcastSyncTaskService> logger) : BackgroundService
{
    private static readonly TimeSpan SyncInterval = TimeSpan.FromSeconds(30);
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(SyncInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await timer.WaitForNextTickAsync(stoppingToken);
                await SyncBroadcastsAsync(stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while syncing broadcasts: {ErrorMessage}; continuing loop", ex.Message);
            }
        }
    }

    private async Task SyncBroadcastsAsync(CancellationToken ct)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        var azuraCastClient = scope.ServiceProvider.GetRequiredService<IAzuraCastClient>();

        var remoteBroadcastsResult = await azuraCastClient.GetBroadcastsAsync();
        if (remoteBroadcastsResult.IsError)
        {
            logger.LogError("Error while fetching remote broadcasts: {ErrorMessage}",
                            remoteBroadcastsResult.Error.ReasonPhrase);
            return;
        }

        var remoteBroadcasts = remoteBroadcastsResult.Value.ToDictionary(broadcast => broadcast.Id);
        var domainBroadcasts =
            await dataContext.Broadcasts.ToDictionaryAsync(broadcast => broadcast.AzuraCastBroadcastId, ct);

        var remoteIds = remoteBroadcasts.Keys;
        var domainIds = domainBroadcasts.Keys;
        var idsToDelete = domainIds.Except(remoteIds);
        var idsToAdd = remoteIds.Except(domainIds);
        var idsToUpdate = domainIds.Intersect(remoteIds);

        foreach (var id in idsToDelete)
        {
            var broadcast = domainBroadcasts.GetValueOrDefault(id);
            broadcast.ShouldNotBeNull();
            dataContext.Broadcasts.Remove(broadcast);
        }

        foreach (var id in idsToAdd)
        {
            var remoteBroadcast = remoteBroadcasts.GetValueOrDefault(id);
            remoteBroadcast.ShouldNotBeNull();
            AddBroadcastToDomain(remoteBroadcast, dataContext);
        }

        foreach (var id in idsToUpdate)
        {
            var domainBroadcast = domainBroadcasts.GetValueOrDefault(id);
            var remoteBroadcast = remoteBroadcasts.GetValueOrDefault(id);
            domainBroadcast.ShouldNotBeNull();
            remoteBroadcast.ShouldNotBeNull();
            
            UpdateBroadcastInDomain(domainBroadcast, remoteBroadcast);
        }

        await dataContext.SaveChangesAsync(ct);
    }

    private void AddBroadcastToDomain(StationStreamerBroadcast remoteBroadcast, DataContext dataContext)
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