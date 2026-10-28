using FastEndpoints;
using LowPressureZone.Data;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.Events;
using LowPressureZone.Identity;
using LowPressureZone.Scheduling.Application.Commands.Schedules.SendReminder;
using LowPressureZone.Scheduling.Application.Jobs;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace LowPressureZone.Scheduling.Application.Consumers.Schedules;

public class HourlySlotReminderSetConsumer(
    IScheduler scheduler,
    DataContext dataContext,
    IdentityContext identityContext,
    ICommandHandler<SendReminderCommand> job) : IEventHandler<HourlySlotReminderSet>
{
    public async Task HandleAsync(HourlySlotReminderSet eventModel, CancellationToken ct)
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
        
        var job = await scheduler.ScheduleJob<IJob<SendReminderCommand>, SendReminderCommand>(new(eventModel.HourlySlotId),
                                                                                        eventModel.NewReminderTime.Value,
                                                                                        OneOffJobOptions.Replacing(jobName),
                                                                                        cancellationToken: ct);
        slot.LinkReminder(job.TriggerKey.Name);
        await dataContext.SaveChangesAsync(ct);
    }
}