using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.PerformerAggregate.Rules;

public class SocialUrlLengthCannotExceed512Rule(string? socialUrl) : IRule
{
    public bool IsBroken() => socialUrl is not null && socialUrl.Trim().Length > 512;

    public RuleError Error => new("Cannot be more than 512 characters", nameof(Performer.SocialUrl));
}