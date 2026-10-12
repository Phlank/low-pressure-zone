using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.HourlySlotTimeRangeObject.Rules;

public class TimeCannotBeInThePastRule(DateTimeOffset startsAt, int duration) : IRule
{
    public bool IsBroken() => startsAt.AddHours(duration) < DateTimeOffset.UtcNow;

    public RuleError Error => new("Cannot be in the past", nameof(HourlySlotTimeRange.Duration)); 
}