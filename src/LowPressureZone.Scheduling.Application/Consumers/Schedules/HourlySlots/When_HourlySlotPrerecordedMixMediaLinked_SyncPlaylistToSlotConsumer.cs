using FastEndpoints;
using LowPressureZone.Core;
using LowPressureZone.Scheduling.Application.Commands.Schedules.HourlySlots.SyncPlaylistTimeToSlot;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity.Events;
using Wolverine.Attributes;

namespace LowPressureZone.Scheduling.Application.Consumers.Schedules.HourlySlots;

[WolverineHandler]
public class When_HourlySlotPrerecordedMixMediaLinked_SyncPlaylistToSlotConsumer(
    ICommandHandler<SyncPlaylistToSlot, Result<string>> sync)
{
    public async Task ConsumeAsync(HourlySlotPrerecordedMixMediaLinked @event)
    {
        var command = new SyncPlaylistToSlot(@event.Id);
        await sync.ExecuteAsync(command, CancellationToken.None);
    }
}