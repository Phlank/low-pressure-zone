using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.PrerecordedMixObject.Rules;

public class MediaRequiredWhenPrerecordedRule(bool isPrerecorded, int? azuraCastMediaId) : IRule
{
    public bool IsBroken() => isPrerecorded && !azuraCastMediaId.HasValue;

    public RuleError Error => new("Required", nameof(PrerecordedMix.AzuraCastMediaId));
}