using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity.Events;

public record PrerecordedMixRemovedFromSlot(Guid HourlySlotId) : IEvent;