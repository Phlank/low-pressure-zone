using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.ScheduleTimeRangeObject.Rules;

public class DurationCannotExceed24HoursRule(int duration) : IRule
{
    public bool IsBroken() => duration > 24;

    public RuleError Error => new("Cannot exceed 24 hours", nameof(ScheduleTimeRange.Duration));
}