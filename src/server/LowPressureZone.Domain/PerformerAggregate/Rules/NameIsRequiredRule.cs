using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.PerformerAggregate.Rules;

public class NameIsRequiredRule(string name) : IRule
{
    public bool IsBroken() => string.IsNullOrWhiteSpace(name);

    public RuleError Error => new RuleError("Required", nameof(Performer.Name));
}