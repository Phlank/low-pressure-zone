using FastEndpoints;
using LowPressureZone.Core;
using LowPressureZone.Scheduling.Application.Commands.Broadcasts.SyncMediaMetadataToArchive;
using LowPressureZone.Scheduling.Domain.BroadcastAggregate.Events;
using Wolverine.Attributes;

namespace LowPressureZone.Scheduling.Application.Consumers.Broadcasts;

[WolverineHandler]
public class When_BroadcastArchived_SyncAzuraCastMediaMetadataConsumer(
    ICommandHandler<SyncMediaMetadataToArchiveCommand, Result<string>> sync)
{
    public async Task ConsumeAsync(BroadcastArchived @event)
    {
        var command = new SyncMediaMetadataToArchiveCommand(@event.Id);
        await sync.ExecuteAsync(command, CancellationToken.None);
    }
}