using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.BroadcastAggregate.Rules;

public class StreamerIdMustBeAboveZeroRule(int azuraCastStreamerId) : IRule
{
    public bool IsBroken() => azuraCastStreamerId <= 0;

    public RuleError Error => new("Must be above zero", nameof(Broadcast.AzuraCastStreamerId));
}