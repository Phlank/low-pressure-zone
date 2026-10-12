using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.Events;

public record PrerecordedMixAddedToSlot(Guid HourlySlotId) : IEvent;