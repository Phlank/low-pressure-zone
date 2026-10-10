using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.ScheduleTimeRangeObject.Rules;

public class DurationCannotExceed24HoursRule(DateTimeOffset startsAt, DateTimeOffset endsAt) : IRule
{
    public bool IsBroken() => endsAt - startsAt > TimeSpan.FromHours(24);

    public RuleError Error => new("Cannot exceed 24 hours", nameof(ScheduleTimeRange.EndsAt));
}