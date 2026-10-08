using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.PerformerAggregate.Rules;

public class SocialUrlMustBeWellFormedIfProvidedRule(string? socialUrl) : IRule
{
    public bool IsBroken() => !string.IsNullOrWhiteSpace(socialUrl)
                              && !Uri.IsWellFormedUriString(socialUrl, UriKind.Absolute);

    public RuleError Error => new("Invalid URL", nameof(Performer.SocialUrl));
}