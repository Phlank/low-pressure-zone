using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.ClashSlotEntity.ClashTimeRangeObject.Rules;

public class DurationCannotExceedFourHoursRule(int duration) : IRule
{
    public bool IsBroken() => duration > 4;

    public RuleError Error => new("Cannot exceed four hours", nameof(ClashTimeRange.Duration));
}