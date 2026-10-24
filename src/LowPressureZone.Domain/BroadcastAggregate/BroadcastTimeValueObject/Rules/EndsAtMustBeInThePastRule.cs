using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.BroadcastAggregate.BroadcastTimeValueObject.Rules;

public class EndsAtMustBeInThePastRule(DateTimeOffset? endsAt) : IRule
{
    public bool IsBroken() => endsAt.HasValue && endsAt > DateTimeOffset.UtcNow;

    public RuleError Error => new("Must be in the past", nameof(BroadcastTime.EndsAt));
}