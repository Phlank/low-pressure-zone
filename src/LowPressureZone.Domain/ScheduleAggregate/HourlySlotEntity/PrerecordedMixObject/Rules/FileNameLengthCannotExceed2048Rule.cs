using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.PrerecordedMixObject.Rules;

public class FileNameLengthCannotExceed2048Rule(string? fileName) : IRule
{
    public bool IsBroken() => fileName is not null && fileName.Length > 2048;

    public RuleError Error => new("Cannot exceed 2048 characters", nameof(PrerecordedMix.UploadedFileName));
}