using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.PrerecordedMixObject.Rules;

public class NameRequiredWhenMediaIdProvidedRule(string? uploadedFileName, int? azuraCastMediaId) : IRule
{
    public bool IsBroken() => uploadedFileName is null && azuraCastMediaId is not null;

    public RuleError Error => new("Required when Media ID is used", nameof(PrerecordedMix.UploadedFileName));
}