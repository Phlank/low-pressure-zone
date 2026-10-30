using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.Events;

public record PrerecordedMixChangedForSlot(Guid HourlySlotId) : IEvent;