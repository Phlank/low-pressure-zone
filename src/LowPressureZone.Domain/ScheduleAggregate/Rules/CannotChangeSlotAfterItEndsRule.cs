using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using LowPressureZone.Core.Interfaces;

namespace LowPressureZone.Domain.ScheduleAggregate.Rules;

public class CannotChangeSlotAfterItEndsRule(ITimeRange slot) : IRule
{
    public bool IsBroken() => slot.EndsAt < DateTime.UtcNow;

    public RuleError Error => new("Cannot change slot after it has ended");
}