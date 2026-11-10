using LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity.Events;
using Quartz;
using Wolverine;
using Wolverine.Attributes;

namespace LowPressureZone.Scheduling.Application.Consumers.Schedules.HourlySlots;

[WolverineHandler]
public class HourlySlotDeletedConsumer(IScheduler scheduler)
{
    public async Task ConsumeAsync(HourlySlotDeleted eventModel, CancellationToken ct)
    {
        if (eventModel.ReminderJobId is not null)
            await scheduler.DeleteJob(new JobKey(eventModel.ReminderJobId), ct);
    }
}