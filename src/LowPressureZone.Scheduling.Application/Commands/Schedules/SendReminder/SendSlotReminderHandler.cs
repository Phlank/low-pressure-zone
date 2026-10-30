using FastEndpoints;
using LowPressureZone.Data;
using LowPressureZone.Identity;
using LowPressureZone.Infrastructure.Emails;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Scheduling.Application.Commands.Schedules.SendReminder;

[RegisterService<ICommandHandler<SendSlotReminderCommand>>(LifeTime.Scoped)]
public class SendSlotReminderHandler(
    IEmailService emailService,
    DataContext dataContext,
    IdentityContext identityContext) : ICommandHandler<SendSlotReminderCommand>
{
    public async Task ExecuteAsync(SendSlotReminderCommand command, CancellationToken ct)
    {
        var slot = await dataContext.HourlySlots
                                    .Include(slot => slot.Performer)
                                    .FirstOrDefaultAsync(slot => slot.Id == command.SlotId, cancellationToken: ct);
        if (slot?.Reminder.ReminderTime is null) return;

        var email = await identityContext.Users
                                         .Where(u => u.Id == slot.Performer.CreatorUserId)
                                         .Select(u => u.Email)
                                         .FirstOrDefaultAsync(ct);
        if (email is null) return;

        await emailService.SendSlotReminderEmailAsync(email,
                                                      slot.Reminder.ReminderTime.Value,
                                                      slot.TimeRange.StartsAt);
    }
}