using FastEndpoints;

namespace LowPressureZone.Scheduling.Application.Commands.Schedules.SendReminder;

public record SendReminderCommand(Guid SlotId) : ICommand;