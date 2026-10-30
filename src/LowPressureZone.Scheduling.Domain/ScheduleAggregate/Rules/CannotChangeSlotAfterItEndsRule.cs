using LowPressureZone.Core;
using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.ScheduleAggregate.Rules;

public class CannotChangeSlotAfterItEndsRule(ITimeRange slot) : IRule
{
    public bool IsBroken() => slot.EndsAt != default && slot.EndsAt < DateTime.UtcNow;

    public RuleError Error => new("Cannot change slot after it has ended");
}