using LowPressureZone.Core.Domain;
using LowPressureZone.Core.Interfaces;

namespace LowPressureZone.Domain.ScheduleAggregate.Rules;

public class SlotsCannotHaveOverlappingTimeRangesRule(List<ITimeRange> existingSlotTimes, ITimeRange newSlotTimes)
    : IRule
{
    public bool IsBroken() => existingSlotTimes.Any(timeRange => timeRange.Overlaps(newSlotTimes));

    public RuleError Error => new("Slot overlaps with other items in the schedule", nameof(ITimeRange.StartsAt));
}