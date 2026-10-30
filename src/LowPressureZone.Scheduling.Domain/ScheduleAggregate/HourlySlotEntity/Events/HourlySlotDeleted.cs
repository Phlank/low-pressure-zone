using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity.Events;

public record HourlySlotDeleted(Guid Id, string? ReminderJobId) : IEvent;