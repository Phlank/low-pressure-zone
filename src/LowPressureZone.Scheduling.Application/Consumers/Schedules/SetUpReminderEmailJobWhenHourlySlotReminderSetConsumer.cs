using FastEndpoints;
using LowPressureZone.Data;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.Events;
using LowPressureZone.Identity;
using LowPressureZone.Scheduling.Application.Commands.Schedules.SendReminder;
using LowPressureZone.Scheduling.Application.Jobs;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Wolverine;

namespace LowPressureZone.Scheduling.Application.Consumers.Schedules;

[RegisterService<SetUpReminderEmailJobWhenHourlySlotReminderSetConsumer>(LifeTime.Scoped)]
public class SetUpReminderEmailJobWhenHourlySlotReminderSetConsumer(
    IScheduler scheduler,
    DataContext dataContext,
    IdentityContext identityContext) : IWolverineHandler
{
    public async Task ConsumeAsync(HourlySlotReminderSet eventModel, CancellationToken ct)
    {
        var jobName = SendSlotReminderJob.JobName(eventModel.HourlySlotId);

        if (eventModel.OldJobId is not null)
            await scheduler.DeleteJob(new JobKey(jobName), ct);

        if (eventModel.NewReminderTime is null)
            return;

        var slot = await dataContext.HourlySlots
                                    .Where(slot => slot.Id == eventModel.HourlySlotId)
                                    .Include(slot => slot.Performer)
                                    .FirstOrDefaultAsync(ct);

        if (slot?.Reminder.ReminderTime == null)
            return;

        var email = await identityContext.Users
                                         .Where(user => user.Id == slot.Performer.CreatorUserId)
                                         .Select(user => user.Email)
                                         .FirstOrDefaultAsync(ct);
        if (email is null) return;

        var job =
            await scheduler.ScheduleJob<IJob<SendReminderCommand>, SendReminderCommand>(new(eventModel.HourlySlotId),
                                                                                        eventModel.NewReminderTime.Value,
                                                                                        OneOffJobOptions.Replacing(jobName),
                                                                                        cancellationToken: ct);
        slot.LinkReminder(job.TriggerKey.Name);
        await dataContext.SaveChangesAsync(ct);
    }
}