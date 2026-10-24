using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.ClashSlotEntity.ClashTimeRangeObject.Rules;

public class TimeCannotBeInThePastRule(DateTimeOffset startsAt, int duration) : IRule
{
    public bool IsBroken() => startsAt.AddHours(duration) < DateTimeOffset.UtcNow;

    public RuleError Error => new("Cannot set time to the past", nameof(ClashSlot.TimeRange.StartsAt)); 
}