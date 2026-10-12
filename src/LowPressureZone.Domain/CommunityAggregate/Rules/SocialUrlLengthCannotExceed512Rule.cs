using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.CommunityAggregate.Rules;

public class SocialUrlLengthCannotExceed512Rule(string socialUrl) : IRule
{
    public bool IsBroken() => socialUrl.Trim().Length > 512;

    public RuleError Error => new("Cannot exceed 512 characters", nameof(Community.SocialUrl));  
}