using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.PrerecordedMixObject.Rules;

public class MediaIdRequiredWhenNameProvidedRule(string? uploadedFileName, int? azuraCastMediaId) : IRule
{
    public bool IsBroken() => uploadedFileName is not null && azuraCastMediaId is null;

    public RuleError Error => new("Required when file name provided", nameof(PrerecordedMix.AzuraCastMediaId));
}