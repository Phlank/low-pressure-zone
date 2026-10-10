using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.ClashSlotEntity.Rules;

public class PerformersMustBeDifferentRule(Guid performerOneId, Guid performerTwoId) : IRule
{
    public bool IsBroken() => performerOneId == performerTwoId;

    public RuleError Error => new("Cannot be the same performer", nameof(ClashSlot.PerformerTwoId));
}