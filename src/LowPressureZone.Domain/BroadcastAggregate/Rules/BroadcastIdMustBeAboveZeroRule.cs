using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.BroadcastAggregate.Rules;

public class BroadcastIdMustBeAboveZeroRule(int azuraCastBroadcastId) : IRule
{
    public bool IsBroken() => azuraCastBroadcastId <= 0;

    public RuleError Error => new("Must be above zero", nameof(Broadcast.AzuraCastBroadcastId));
}