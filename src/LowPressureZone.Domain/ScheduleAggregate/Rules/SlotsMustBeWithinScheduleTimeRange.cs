using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using LowPressureZone.Domain.ScheduleAggregate.ScheduleTimeRangeObject;

namespace LowPressureZone.Domain.ScheduleAggregate.Rules;

public class SlotsMustBeWithinScheduleTimeRange(ScheduleTimeRange scheduleTimeRange, List<ITimeRange> slots) : IRule
{
    public bool IsBroken() => slots.Any(slot => !slot.IsWithin(scheduleTimeRange));

    public RuleError Error => new("Must be within schedule time range", nameof(ITimeRange.StartsAt));
}