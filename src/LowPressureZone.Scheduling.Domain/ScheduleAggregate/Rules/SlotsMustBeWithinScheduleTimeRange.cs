using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.ScheduleTimeRangeObject;

namespace LowPressureZone.Scheduling.Domain.ScheduleAggregate.Rules;

public class SlotsMustBeWithinScheduleTimeRange(ScheduleTimeRange scheduleTimeRange, List<ITimeRange> slots) : IRule
{
    public bool IsBroken() => slots.Any(slot => !slot.IsWithin(scheduleTimeRange));

    public RuleError Error => new("Must be within schedule time range", nameof(ITimeRange.StartsAt));
}