using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Adapter.AzuraCast.Exceptions;
using LowPressureZone.Adapter.AzuraCast.Mappers;
using LowPressureZone.Api.Services.AzuraCast;
using LowPressureZone.Core;
using LowPressureZone.Data;
using LowPressureZone.Scheduling.Application.Extensions;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity.Events;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.EntityFrameworkCore;

namespace LowPressureZone.Scheduling.Application.Commands.Schedules.HourlySlots.SyncMediaMetadataToSlot;

[RegisterService<ICommandHandler<SyncMediaMetadataToSlot, Result<string>>>(LifeTime.Scoped)]
public class SyncMediaMetadataToSlotHandler(IDbContextOutbox<DataContext> outbox, IAzuraCastClient azuraCast)
    : ICommandHandler<SyncMediaMetadataToSlot, Result<string>>
{
    public async Task<Result<string>> ExecuteAsync(SyncMediaMetadataToSlot command, CancellationToken ct)
    {
        var slot = await outbox.DbContext
                               .HourlySlots
                               .FirstOrDefaultAsync(s => s.Id == command.SlotId, CancellationToken.None);
        if (slot is null)
            return Results.Err($"HourlySlot with Id {command.SlotId} not found.");

        if (slot.PrerecordedMix?.UploadedMedia is null)
            return Results.Err($"HourlySlot with Id {command.SlotId} has no uploaded media.");

        if (slot.PrerecordedMix.UploadedMedia.AzuraCastMediaId is not null)
            return Results.Err($"HourlySlot with Id {command.SlotId} already has AzuraCastMediaId.");

        var fileName = slot.PrerecordedMix.UploadedMedia.FileName;
        var listMedia = await azuraCast.ListMediaAsync(AzuraCastMediaDirectory.Prerecords, fileName);

        var media = listMedia.ValueOrDefault?.FirstOrDefault(m => m.Path == slot.PrerecordedMix.UploadedMedia.FilePath);
        if (media is null)
            throw new AzuraCastStateException("Media not found for slot");

        if (media.Media is null)
            throw new AzuraCastStateException("Media not yet processed for slot.");

        if (slot.PrerecordedMix.UploadedMedia.AzuraCastMediaId is null)
        {
            var link = slot.PrerecordedMix.UploadedMedia.LinkAzuraCastMedia(media.Media.Id);
            if (link.IsError)
                return Results.Err($"Failed to link AzuraCast media for slot {command.SlotId}: {link.Error}");
            await outbox.PublishAllAsync(link.Events);
            await outbox.PublishAsync(new HourlySlotPrerecordedMixMediaLinked(command.SlotId));
        }

        var putRequest = StationMediaMapper.ToRequest(media.Media);
        putRequest.Artist = slot.Performer.Name;
        putRequest.Title = slot.TrackTitle;

        var putResult = await azuraCast.PutMediaAsync(media.Media.Id, putRequest);
        if (putResult.IsError)
            throw new
                IntegrationException($"Failed to update media metadata in AzuraCast for slot {command.SlotId}: {putResult.Error}");
        
        await outbox.SaveChangesAndFlushMessagesAsync(CancellationToken.None);
        return Results.Ok<string>();
    }
}