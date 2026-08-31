using LowPressureZone.Core.Domain;
using LowPressureZone.Domain.BroadcastAggregate.BroadcastTimeValueObject.Rules;

namespace LowPressureZone.Domain.BroadcastAggregate.BroadcastTimeValueObject;

public readonly record struct BroadcastTime
{
    public DateTimeOffset StartsAt { get; private init; }
    public DateTimeOffset? EndsAt { get; private init; }

    public static DomainResult<BroadcastTime> Create(DateTimeOffset startsAt, DateTimeOffset? endsAt) =>
        Rule.ApplyIntoResult(new BroadcastTime()
                             {
                                 StartsAt = startsAt,
                                 EndsAt = endsAt
                             },
                             new StartsAtMustBeInThePastRule(startsAt),
                             new EndsAtMustBeInThePastRule(endsAt),
                             new EndMustBeLaterThanStartIfProvidedRule(startsAt, endsAt));
}