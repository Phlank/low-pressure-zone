using LowPressureZone.Core.Domain;
using LowPressureZone.Core.Interfaces;
using LowPressureZone.Domain.ScheduleAggregate.ScheduleTimeRangeObject;

namespace LowPressureZone.Domain.ScheduleAggregate.Rules;

public class SlotMustBeWithinScheduleTimeRange(ScheduleTimeRange scheduleTimeRange, ITimeRange slot) : IRule
{
    public bool IsBroken() => slot.IsWithin(scheduleTimeRange);

    public RuleError Error => new("Must be within schedule time range", nameof(ITimeRange.StartsAt));
}