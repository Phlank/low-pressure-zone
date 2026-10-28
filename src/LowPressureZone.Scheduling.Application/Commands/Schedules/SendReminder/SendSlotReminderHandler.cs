using FastEndpoints;
using LowPressureZone.Data;
using LowPressureZone.Identity;
using LowPressureZone.Infrastructure.Emails;

namespace LowPressureZone.Scheduling.Application.Commands.Schedules.SendReminder;

[RegisterService<ICommandHandler<SendReminderCommand>>(LifeTime.Scoped)]
public class SendSlotReminderHandler(IEmailService emailService, DataContext dataContext, IdentityContext identityContext) : ICommandHandler<SendReminderCommand>
{
    public Task ExecuteAsync(SendReminderCommand command, CancellationToken ct)
    {
        var slot = dataContext.HourlySlots.FirstOrDefault(slot => slot.Id == command.SlotId);
        if (slot is null) return Task.CompletedTask;

        var performer = dataContext.Performers.FirstOrDefault(performer => performer.Id == slot.PerformerId);
        if (performer is null) return Task.CompletedTask;

        var user = identityContext.Users.FirstOrDefault(user => user.Id == performer.CreatorUserId);
        if (user is null) return Task.CompletedTask;

        return emailService.SendSlotReminderEmailAsync(user.Email!, slot.TimeRange.StartsAt - slot.Reminder.ReminderTime!.Value, slot.TimeRange.StartsAt);
    }
}