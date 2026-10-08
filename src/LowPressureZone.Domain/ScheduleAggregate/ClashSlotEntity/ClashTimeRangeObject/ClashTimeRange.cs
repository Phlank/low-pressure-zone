using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using LowPressureZone.Core.Interfaces;
using LowPressureZone.Domain.ScheduleAggregate.ClashSlotEntity.ClashTimeRangeObject.Rules;

namespace LowPressureZone.Domain.ScheduleAggregate.ClashSlotEntity.ClashTimeRangeObject;

public readonly record struct ClashTimeRange : ITimeRange
{
    public DateTimeOffset StartsAt { get; private init; }
    public int Duration { get; private init; }
    public DateTimeOffset EndsAt { get; private init; }
    public TimeSpan TimeSpan => TimeSpan.FromHours(Duration);

    public static DomainResult<ClashTimeRange> Create(DateTimeOffset startsAt, int duration) =>
        Rule.ApplyIntoResult(new ClashTimeRange
                                          {
                                              StartsAt = startsAt,
                                              Duration = duration,
                                              EndsAt = startsAt.AddHours(duration)
                                          },
                                          new DurationMustBeAtLeastOneHourRule(duration),
                                          new DurationCannotExceedFourHoursRule(duration),
                                          new TimeCannotBeInThePastRule(startsAt, duration));
}