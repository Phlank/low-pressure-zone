using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.CommunityAggregate.Rules;

public class SocialUrlMustBeWellFormedRule(string socialUrl) : IRule
{
    public bool IsBroken() => Uri.IsWellFormedUriString(socialUrl, UriKind.Absolute);

    public RuleError Error => new("Invalid URL", nameof(Community.SocialUrl)); 
}