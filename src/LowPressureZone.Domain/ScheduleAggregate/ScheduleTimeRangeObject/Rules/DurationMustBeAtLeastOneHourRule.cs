using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.ScheduleTimeRangeObject.Rules;

public class DurationMustBeAtLeastOneHourRule(DateTimeOffset startsAt, DateTimeOffset endsAt) : IRule
{

    public bool IsBroken() => endsAt - startsAt < TimeSpan.FromHours(1);

    public RuleError Error => new("Must be at least one hour", nameof(ScheduleTimeRange.EndsAt));
}