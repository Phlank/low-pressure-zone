using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.AllowedScheduleSlotTypesObject.Rules;

public class AtLeastOneSlotTypeMustBeAllowedRule(bool isHourlyAllowed, bool isClashAllowed) : IRule
{
    public bool IsBroken() => !isHourlyAllowed && !isClashAllowed;

    public RuleError Error => new("At least one slot type must be allowed",
                                  nameof(AllowedScheduleSlotTypes.IsHourlyAllowed));
}