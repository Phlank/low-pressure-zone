using FastEndpoints;
using Hangfire;
using LowPressureZone.Data;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.Events;
using LowPressureZone.Identity;
using LowPressureZone.Scheduling.Application.Commands.Schedules.SendReminder;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Scheduling.Application.Consumers.Schedules;

public class HourlySlotReminderSetConsumer(
    IBackgroundJobClient backgroundJobClient,
    DataContext dataContext,
    IdentityContext identityContext,
    ICommandHandler<SendReminderCommand> job) : IEventHandler<HourlySlotReminderSet>
{
    public async Task HandleAsync(HourlySlotReminderSet eventModel, CancellationToken ct)
    {
        if (eventModel.OldJobId is not null)
        {
            backgroundJobClient.Delete(eventModel.OldJobId);
        }

        var slot = await dataContext.HourlySlots
                                    .Where(slot => slot.Id == eventModel.HourlySlotId)
                                    .Include(slot => slot.Performer)
                                    .FirstOrDefaultAsync(ct);
        if (slot is null || !slot.Reminder.ReminderTime.HasValue) return;

        var email = await identityContext.Users
                                        .Where(user => user.Id == slot.Performer.CreatorUserId)
                                        .Select(user => user.Email)
                                        .FirstOrDefaultAsync(ct);
        if (email is null) return;
        
        var jobId = backgroundJobClient.Schedule(() => job.ExecuteAsync(new(slot.Id), ct), slot.Reminder.ReminderTime.Value);
        
        slot.LinkReminder(jobId);
        await dataContext.SaveChangesAsync(ct);
    }
}