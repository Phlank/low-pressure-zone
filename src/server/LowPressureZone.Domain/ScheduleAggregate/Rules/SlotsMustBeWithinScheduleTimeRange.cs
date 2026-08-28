using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using LowPressureZone.Core.Interfaces;
using LowPressureZone.Domain.ScheduleAggregate.ScheduleTimeRangeObject;

namespace LowPressureZone.Domain.ScheduleAggregate.Rules;

public class SlotsMustBeWithinScheduleTimeRange(ScheduleTimeRange scheduleTimeRange, List<ITimeRange> slots) : IRule
{
    public bool IsBroken() => slots.All(slot => slot.IsWithin(scheduleTimeRange));

    public RuleError Error => new("Must be within schedule time range", nameof(ITimeRange.StartsAt));
}