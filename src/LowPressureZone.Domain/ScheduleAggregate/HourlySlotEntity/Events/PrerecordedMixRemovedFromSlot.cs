using FastEndpoints;

namespace LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.Events;

public record PrerecordedMixRemovedFromSlot(Guid HourlySlotId) : IEvent;