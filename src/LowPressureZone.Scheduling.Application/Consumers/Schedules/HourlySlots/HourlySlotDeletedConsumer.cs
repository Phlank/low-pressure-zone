using LowPressureZone.Data;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity.Events;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Wolverine;

namespace LowPressureZone.Scheduling.Application.Consumers.Schedules.HourlySlots;

public class HourlySlotDeletedConsumer(IScheduler scheduler) : IWolverineHandler
{
    public async Task ConsumeAsync(HourlySlotDeleted eventModel, CancellationToken ct)
    {
        if (eventModel.ReminderJobId is not null)
            await scheduler.DeleteJob(new JobKey(eventModel.ReminderJobId), ct);
    }
}