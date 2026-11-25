using System.Globalization;
using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Adapter.AzuraCast.Exceptions;
using LowPressureZone.Adapter.AzuraCast.Mappers;
using LowPressureZone.Api.Services.AzuraCast;
using LowPressureZone.Core;
using LowPressureZone.Data;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.EntityFrameworkCore;

namespace LowPressureZone.Scheduling.Application.Commands.Broadcasts.SyncMediaMetadataToArchive;

[RegisterService<ICommandHandler<SyncMediaMetadataToArchiveCommand, Result<string>>>(LifeTime.Scoped)]
public class SyncMediaMetadataToArchiveHandler(IDbContextOutbox<DataContext> outbox, IAzuraCastClient azuraCast)
    : ICommandHandler<SyncMediaMetadataToArchiveCommand, Result<string>>
{
    public async Task<Result<string>> ExecuteAsync(SyncMediaMetadataToArchiveCommand command, CancellationToken ct)
    {
        var broadcast = await outbox.DbContext.Broadcasts.FirstOrDefaultAsync(b => b.Id == command.Id, ct);
        if (broadcast is null)
            return Results.Err("Broadcast not found");

        if (broadcast.UploadedMediaId is null || broadcast.UploadedMedia is null)
            return Results.Err("Broadcast has no uploaded media");

        if (broadcast.UploadedMedia.AzuraCastMediaId is not null)
            return Results.Err("Uploaded media already linked to AzuraCast media");
        
        var externalBroadcasts = await azuraCast.GetBroadcastsAsync(broadcast.AzuraCastStreamerId);
        if (externalBroadcasts.IsError)
            throw new AzuraCastStateException("Failed to get broadcasts from AzuraCast: " + externalBroadcasts.Error);
        var externalBroadcast = externalBroadcasts.Value.FirstOrDefault(eb => eb.Id == broadcast.AzuraCastBroadcastId);
        if (externalBroadcast is null)
            throw new AzuraCastStateException("Broadcast not found in AzuraCast");

        var listMedia = await azuraCast.ListMediaAsync(AzuraCastMediaDirectory.Archives, 
                                                       broadcast.UploadedMedia.FileName);
        if (listMedia.IsError)
            throw new IntegrationException("Failed to list media from AzuraCast: " + listMedia.Error);
        
        var media = listMedia.ValueOrDefault?.FirstOrDefault(m => m.Path == broadcast.UploadedMedia.FilePath);
        if (media is null)
            throw new AzuraCastStateException("Media not found in AzuraCast" + listMedia.Error);
        
        if (media.Media is null)
            throw new AzuraCastStateException("Media not yet processed in AzuraCast");

        if (broadcast.UploadedMedia.AzuraCastMediaId is null)
        {
            var link = broadcast.UploadedMedia.LinkAzuraCastMedia(media.Media.Id);
            if (link.IsError)
                return Results.Err("Failed to link AzuraCast media: " + link.Error);
            await outbox.PublishAllAsync(link.Events);
        }
        
        var putRequest = StationMediaMapper.ToRequest(media.Media);
        putRequest.Artist = broadcast.AzuraCastStreamerDisplayName;
        putRequest.Title = externalBroadcast.TimestampStart.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        var putResult = await azuraCast.PutMediaAsync(media.Media.Id, putRequest);
        if (putResult.IsError)
            throw new IntegrationException($"Failed to update media metadata in AzuraCast for broadcast {command.Id}: {putResult.Error.ReasonPhrase}");

        await outbox.SaveChangesAndFlushMessagesAsync(CancellationToken.None);
        return Results.Ok<string>();
    }
}