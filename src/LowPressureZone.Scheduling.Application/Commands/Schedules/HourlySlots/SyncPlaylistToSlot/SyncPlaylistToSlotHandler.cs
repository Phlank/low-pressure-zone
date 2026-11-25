using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Core;
using LowPressureZone.Data;
using LowPressureZone.Infrastructure.Abstraction;
using LowPressureZone.Infrastructure.Events;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.EntityFrameworkCore;

namespace LowPressureZone.Scheduling.Application.Commands.Schedules.HourlySlots.SyncPlaylistToSlot;

[RegisterService<ICommandHandler<SyncPlaylistTimeToSlot.SyncPlaylistToSlot, Result<string>>>(LifeTime.Scoped)]
public class SyncPlaylistToSlotHandler(
    IDbContextOutbox<DataContext> outbox,
    IAzuraCastClient azuraCast,
    IConverter<HourlySlot, StationPlaylist> converter)
    : ICommandHandler<SyncPlaylistTimeToSlot.SyncPlaylistToSlot, Result<string>>
{
    public async Task<Result<string>> ExecuteAsync(SyncPlaylistTimeToSlot.SyncPlaylistToSlot command, CancellationToken ct)
    {
        var slot = await outbox.DbContext.HourlySlots.FirstOrDefaultAsync(s => s.Id == command.Id, ct);
        if (slot is null)
            return Results.Err("Hourly slot not found");

        if (slot.PrerecordedMix?.UploadedMedia?.AzuraCastMediaId is null)
            return Results.Err("Hourly slot has no corresponding media file in AzuraCast yet");

        var playlist = converter.Convert(slot);
        if (playlist.IsError)
            return Results.Err("Unable to convert slot to AzuraCast playlist: " + playlist.Error);

        var existingPlaylistId = slot.PrerecordedMix.UploadedMedia.AzuraCastPlaylistId;
        if (!existingPlaylistId.HasValue)
        {
            var postResult = await azuraCast.PostPlaylistAsync(playlist.Value);
            if (postResult.IsError)
            {
                await outbox.PublishAsync(new NotifiableCircumstanceOccurred(nameof(SyncPlaylistToSlotHandler),
                                                                             "Failed to POST to AzuraCast"));
                return Results.Err("Unable to create playlist for slot in AzuraCast: " +
                                   (postResult.Error.ReasonPhrase ?? postResult.Error.StatusCode.ToString()));
            }

            var link = slot.PrerecordedMix.UploadedMedia.LinkAzuraCastPlaylist(postResult.Value);
            await outbox.PublishAllAsync(link.Events);
            await outbox.SaveChangesAndFlushMessagesAsync(CancellationToken.None);
            return Results.Ok<string>();
        }

        playlist.Value.Id = existingPlaylistId.Value;
        var putResult = await azuraCast.PutPlaylistAsync(playlist.Value);
        if (putResult.IsError)
        {
            await outbox.PublishAsync(new NotifiableCircumstanceOccurred(nameof(SyncPlaylistToSlotHandler),
                                                                         "Failed to PUT to AzuraCast"));
            return Results.Err("Unable to update playlist for slot in AzuraCast: " +
                               (putResult.Error.ReasonPhrase ?? putResult.Error.StatusCode.ToString()));
        }
        return Results.Ok<string>();
    }
}