using FastEndpoints;
using LowPressureZone.Core;
using LowPressureZone.Data;
using LowPressureZone.Scheduling.Application.Commands.Schedules.HourlySlots.SyncMediaMetadataToSlot;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity.Events;
using Wolverine.Attributes;
using Wolverine.EntityFrameworkCore;

namespace LowPressureZone.Scheduling.Application.Consumers.Schedules.HourlySlots;

[WolverineHandler]
public class When_HourlySlotPrerecordedMixAdded_SyncMediaMetadataToSlotConsumer(
    ICommandHandler<SyncMediaMetadataToSlot, Result<string>> sync)
{
    public async Task ConsumeAsync(HourlySlotPrerecordedMixAdded @event)
    {
        var command = new SyncMediaMetadataToSlot(@event.Id);
        await sync.ExecuteAsync(command, CancellationToken.None);
    }
}