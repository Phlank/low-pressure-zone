using LowPressureZone.Core.Domain;
using LowPressureZone.Domain.ScheduleAggregate.AllowedScheduleSlotTypesObject;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity;

namespace LowPressureZone.Domain.ScheduleAggregate.Rules;

public class NoHourlySlotsWhenNotAllowed(List<HourlySlot> slots, bool isHourlyAllowed) : IRule
{
    public bool IsBroken() => slots.Count > 0 && !isHourlyAllowed;

    public RuleError Error => new("Cannot restrict hourly slots when schedule has them",
                                  nameof(AllowedScheduleSlotTypes.IsHourlyAllowed));
}