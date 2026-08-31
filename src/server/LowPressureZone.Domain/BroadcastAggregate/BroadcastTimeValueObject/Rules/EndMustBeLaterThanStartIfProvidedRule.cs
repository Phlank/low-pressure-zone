using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.BroadcastAggregate.BroadcastTimeValueObject.Rules;

public class EndMustBeLaterThanStartIfProvidedRule(DateTimeOffset startsAt, DateTimeOffset? endsAt) : IRule
{
    public bool IsBroken() => endsAt.HasValue && endsAt <= startsAt;

    public RuleError Error => new("Must be later than start", nameof(BroadcastTime.EndsAt));
}