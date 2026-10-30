using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.Events;

public record PrerecordedMixRemovedFromSlot(Guid HourlySlotId) : IEvent;