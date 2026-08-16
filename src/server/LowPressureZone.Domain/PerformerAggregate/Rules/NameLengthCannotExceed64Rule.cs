using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.PerformerAggregate.Rules;

public class NameLengthCannotExceed64Rule(string name) : IRule
{
    public bool IsBroken() => name.Length > 64;

    public RuleError Error => new RuleError("Cannot be more than 64 characters", nameof(Performer.Name));
}