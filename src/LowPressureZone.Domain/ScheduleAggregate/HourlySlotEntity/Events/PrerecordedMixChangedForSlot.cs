using FastEndpoints;

namespace LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.Events;

public record PrerecordedMixChangedForSlot(Guid HourlySlotId) : IEvent;