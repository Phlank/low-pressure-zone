using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.ScheduleTimeRangeObject.Rules;

namespace LowPressureZone.Scheduling.Domain.ScheduleAggregate.ScheduleTimeRangeObject;

public readonly record struct ScheduleTimeRange : ITimeRange
{
    public DateTimeOffset StartsAt { get; private init; }
    public DateTimeOffset EndsAt { get; private init; }
    public TimeSpan TimeSpan => EndsAt - StartsAt;

    public static DomainResult<ScheduleTimeRange> Create(DateTimeOffset startsAt, DateTimeOffset endsAt) =>
        Rule.ApplyIntoResult(new ScheduleTimeRange
                                          {
                                              StartsAt = startsAt,
                                              EndsAt = endsAt
                                          },
                                          new DurationMustBeAtLeastOneHourRule(startsAt, endsAt),
                                          new DurationCannotExceed24HoursRule(startsAt, endsAt));
}