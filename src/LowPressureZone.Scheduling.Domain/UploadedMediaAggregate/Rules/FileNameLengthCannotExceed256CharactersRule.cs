using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.UploadedMediaAggregate.Rules;

public class FileNameLengthCannotExceed256CharactersRule(UploadedMedia value) : IRule
{
    public bool IsBroken() => value.FileName.Length > 256;

    public RuleError Error => new("Cannot exceed 256 characters", nameof(UploadedMedia.FileName));
}