using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Core;
using LowPressureZone.Data;
using LowPressureZone.Scheduling.Application.Commands.UploadedMedia.AddMediaToPlaylist;
using LowPressureZone.Scheduling.Domain.UploadedMediaAggregate.Events;
using Microsoft.EntityFrameworkCore;
using Wolverine.Attributes;
using Wolverine.EntityFrameworkCore;

namespace LowPressureZone.Scheduling.Application.Consumers.UploadedMedia;

[WolverineHandler]
public class When_UploadedMediaLinkedToPlaylist_AddMediaToPlaylistConsumer(
    ICommandHandler<AddMediaToPlaylist, Result<string>> addMediaToPlaylist)
{
    public async Task ConsumeAsync(UploadedMediaLinkedToPlaylist @event)
    {
        var command = new AddMediaToPlaylist(@event.Id);
        await addMediaToPlaylist.ExecuteAsync(command, CancellationToken.None);
    }
}