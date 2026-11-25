using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity.PrerecordedMixEntity.Rules;

public class UploadedFileNameIsRequiredRule(PrerecordedMix prerecordedMix) : IRule
{
    public bool IsBroken() => string.IsNullOrWhiteSpace(prerecordedMix.UploadedFileName);

    public RuleError Error => new("Required", nameof(PrerecordedMix.UploadedFileName));
}