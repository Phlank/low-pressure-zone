using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.PrerecordedMixObject.Rules;

public class FileNameRequiredWhenPrerecordedRule(bool isPrerecorded, string? fileName) : IRule
{
    public bool IsBroken() => isPrerecorded && !string.IsNullOrWhiteSpace(fileName);

    public RuleError Error => new("Required", nameof(PrerecordedMix.UploadedFileName));
}