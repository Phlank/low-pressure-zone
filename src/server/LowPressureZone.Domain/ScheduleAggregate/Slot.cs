using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity;
using LowPressureZone.Domain.ScheduleAggregate.ClashSlotEntity;

namespace LowPressureZone.Domain.ScheduleAggregate;

public union Slot(HourlySlot, ClashSlot);