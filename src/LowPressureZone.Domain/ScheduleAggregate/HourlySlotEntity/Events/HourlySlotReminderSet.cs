using FastEndpoints;

namespace LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.Events;

public readonly record struct HourlySlotReminderSet(
    Guid HourlySlotId,
    string? OldJobId,
    DateTimeOffset? NewReminderTime) : IEvent;