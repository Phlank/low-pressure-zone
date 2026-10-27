using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.Events;

public record HourlySlotReminderSet(Guid HourlySlotId, string? OldJobId) : IEvent
{
}