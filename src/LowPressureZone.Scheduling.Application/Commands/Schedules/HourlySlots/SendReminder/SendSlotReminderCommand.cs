using FastEndpoints;

namespace LowPressureZone.Scheduling.Application.Commands.Schedules.HourlySlots.SendReminder;

public record SendSlotReminderCommand(Guid SlotId) : ICommand;