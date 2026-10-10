using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.CommunityAggregate.Rules;

public class NameLengthCannotExceed128Rule(string name) : IRule
{
    public bool IsBroken() => name.Trim().Length > 128;

    public RuleError Error => new("Name cannot exceed 128 characters", nameof(Community.Name));   
}