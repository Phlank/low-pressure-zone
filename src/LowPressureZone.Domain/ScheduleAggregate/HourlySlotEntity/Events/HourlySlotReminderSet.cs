using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.Events;

public readonly record struct HourlySlotReminderSet(Guid HourlySlotId, string? OldJobId) : IEvent;