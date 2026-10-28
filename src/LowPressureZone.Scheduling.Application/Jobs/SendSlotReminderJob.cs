using FastEndpoints;
using LowPressureZone.Data;
using LowPressureZone.Identity;
using LowPressureZone.Infrastructure.Emails;
using LowPressureZone.Scheduling.Application.Commands.Schedules.SendReminder;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace LowPressureZone.Scheduling.Application.Jobs;

[RegisterService<IJob<SendReminderCommand>>(LifeTime.Scoped)]
public class SendSlotReminderJob(DataContext dataContext, IdentityContext identityContext, IEmailService emailService)
    : IJob<SendReminderCommand>
{
    public async ValueTask Execute(IJobExecutionContext context,
                                   SendReminderCommand input,
                                   CancellationToken cancellationToken = new())
    {
        var slot = await dataContext.HourlySlots.FirstOrDefaultAsync(slot => slot.Id == input.SlotId,
                                                                     cancellationToken);
        if (slot?.Reminder.ReminderTime is null)
            return;

        var email = await identityContext.Users
                                         .Where(user => user.Id == slot.Performer.CreatorUserId)
                                         .Select(user => user.Email)
                                         .FirstOrDefaultAsync(cancellationToken);
        if (email is null)
            return;

        await emailService.SendSlotReminderEmailAsync(email,
                                                      slot.Reminder.ReminderTime.Value,
                                                      slot.TimeRange.StartsAt);
    }

    public static string JobName(Guid slotId) => $"{nameof(SendSlotReminderJob)}-{slotId}";
}