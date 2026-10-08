using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.BroadcastAggregate.BroadcastTimeValueObject.Rules;

public class StartsAtMustBeInThePastRule(DateTimeOffset startsAt) : IRule
{
    public bool IsBroken() => startsAt > DateTimeOffset.UtcNow;

    public RuleError Error => new("Must be in the past", nameof(BroadcastTime.StartsAt));
}