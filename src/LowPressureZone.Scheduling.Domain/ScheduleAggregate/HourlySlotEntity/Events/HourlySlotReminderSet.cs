using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity.Events;

public readonly record struct HourlySlotReminderSet(
    Guid Id,
    string? OldJobId,
    DateTimeOffset? NewReminderTime) : IEvent;