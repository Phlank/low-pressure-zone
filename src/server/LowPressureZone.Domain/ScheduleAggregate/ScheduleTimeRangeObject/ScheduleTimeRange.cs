using LowPressureZone.Core.Domain;
using LowPressureZone.Core.Interfaces;
using LowPressureZone.Domain.ScheduleAggregate.ScheduleTimeRangeObject.Rules;

namespace LowPressureZone.Domain.ScheduleAggregate.ScheduleTimeRangeObject;

public readonly record struct ScheduleTimeRange : ITimeRange
{
    public DateTimeOffset StartsAt { get; private init; }
    public int Duration { get; private init; }
    public DateTimeOffset EndsAt { get; private init; }
    public TimeSpan TimeSpan => TimeSpan.FromHours(Duration);

    public static DomainResult<ScheduleTimeRange> Create(DateTimeOffset startsAt, int duration) =>
        Rule.ApplyIntoResult(new ScheduleTimeRange
                             {
                                 StartsAt = startsAt,
                                 Duration = duration,
                                 EndsAt = startsAt.AddHours(duration)
                             },
                             new DurationMustBeAtLeastOneHourRule(duration),
                             new DurationCannotExceed24HoursRule(duration));
}