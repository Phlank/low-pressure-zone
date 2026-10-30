using LowPressureZone.Core.Domain;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.AllowedScheduleSlotTypesObject;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.ClashSlotEntity;

namespace LowPressureZone.Scheduling.Domain.ScheduleAggregate.Rules;

public class NoClashSlotsWhenNotAllowed(List<ClashSlot> slots, bool isClashAllowed) : IRule
{
    public bool IsBroken() => slots.Count > 0 && !isClashAllowed;

    public RuleError Error => new("Cannot restrict clash slots when schedule has them",
                                  nameof(AllowedScheduleSlotTypes.IsClashAllowed));
}