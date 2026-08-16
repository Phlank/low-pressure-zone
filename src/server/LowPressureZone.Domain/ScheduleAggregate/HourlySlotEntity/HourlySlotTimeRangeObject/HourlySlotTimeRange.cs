using LowPressureZone.Core.Domain;
using LowPressureZone.Core.Interfaces;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.HourlySlotTimeRangeObject.Rules;

namespace LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.HourlySlotTimeRangeObject;

public readonly record struct HourlySlotTimeRange : ITimeRange
{
    public DateTimeOffset StartsAt { get; private init; }
    public int Duration { get; private init; }
    public DateTimeOffset EndsAt { get; private init; }
    public TimeSpan TimeSpan => EndsAt - StartsAt;

    public static DomainResult<HourlySlotTimeRange> Create(DateTimeOffset startsAt, int duration) =>
        Rule.ApplyIntoResult(new HourlySlotTimeRange
                             {
                                 StartsAt = startsAt,
                                 Duration = duration,
                                 EndsAt = startsAt.AddHours(duration)
                             },
                             new DurationCannotExceedThreeHoursRule(duration),
                             new DurationMustBeAtLeastOneHourRule(duration));
}