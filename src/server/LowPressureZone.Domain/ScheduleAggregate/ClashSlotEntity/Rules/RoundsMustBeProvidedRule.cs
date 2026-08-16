using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.ClashSlotEntity.Rules;

public class RoundsMustBeProvidedRule(List<string> rounds) : IRule
{
    public bool IsBroken() => rounds.Any(round => !string.IsNullOrWhiteSpace(round));

    public RuleError Error => new("Cannot be empty", nameof(ClashSlot.Rounds));
}