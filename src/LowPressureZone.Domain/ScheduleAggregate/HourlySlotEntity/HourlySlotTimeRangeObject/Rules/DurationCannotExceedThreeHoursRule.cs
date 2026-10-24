using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.HourlySlotTimeRangeObject.Rules;

public class DurationCannotExceedThreeHoursRule(int duration) : IRule
{
    public bool IsBroken() => duration > 3;

    public RuleError Error => new("Cannot exceed three hours", nameof(HourlySlotTimeRange.Duration));
}