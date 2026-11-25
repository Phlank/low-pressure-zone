using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity.PrerecordedMixEntity.Rules;

public class UploadedFileNameLengthMaxIs2048Rule(PrerecordedMix prerecordedMix) : IRule
{
    public bool IsBroken() => prerecordedMix.UploadedFileName.Length > 2048;

    public RuleError Error => new("Max length is 2048", nameof(PrerecordedMix.UploadedFileName));
}