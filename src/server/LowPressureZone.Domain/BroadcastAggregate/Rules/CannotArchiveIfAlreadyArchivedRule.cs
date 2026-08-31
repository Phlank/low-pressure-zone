using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.BroadcastAggregate.Rules;

public class CannotArchiveIfAlreadyArchivedRule(Broadcast broadcast) : IRule
{
    public bool IsBroken() => broadcast.IsArchived;

    public RuleError Error => new("Cannot archive an already archived broadcast", nameof(Broadcast.IsArchived));
}