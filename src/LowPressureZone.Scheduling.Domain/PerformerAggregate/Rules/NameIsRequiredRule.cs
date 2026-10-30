using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.PerformerAggregate.Rules;

public class NameIsRequiredRule(string name) : IRule
{
    public bool IsBroken() => string.IsNullOrWhiteSpace(name);

    public RuleError Error => new("Required", nameof(Performer.Name));
}