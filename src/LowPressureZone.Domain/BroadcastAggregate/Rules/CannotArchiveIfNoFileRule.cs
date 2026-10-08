using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.BroadcastAggregate.Rules;

public class CannotArchiveIfNoFileRule(Broadcast broadcast) : IRule
{
    public bool IsBroken() => !broadcast.HasFile;

    public RuleError Error => new("Cannot archive a broadcast without a file", nameof(Broadcast.HasFile));
}