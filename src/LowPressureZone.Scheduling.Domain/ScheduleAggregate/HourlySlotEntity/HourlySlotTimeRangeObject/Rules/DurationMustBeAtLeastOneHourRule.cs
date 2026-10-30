using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity.HourlySlotTimeRangeObject.Rules;

public class DurationMustBeAtLeastOneHourRule(int duration) : IRule
{
    public bool IsBroken() => duration < 1;

    public RuleError Error => new("Must be at least one hour", nameof(HourlySlotTimeRange.Duration));
}