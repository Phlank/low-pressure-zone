using FastEndpoints;

namespace LowPressureZone.Scheduling.Application.Commands.Schedules.SendReminder;

public record SendSlotReminderCommand(Guid SlotId) : ICommand;