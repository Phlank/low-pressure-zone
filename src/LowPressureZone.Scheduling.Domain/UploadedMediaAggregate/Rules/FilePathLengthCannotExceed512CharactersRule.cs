using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.UploadedMediaAggregate.Rules;

public class FilePathLengthCannotExceed512CharactersRule(UploadedMedia value) : IRule
{
    public bool IsBroken() => value.FilePath.Length > 512;

    public RuleError Error => new("Cannot exceed 512 characters", nameof(UploadedMedia.FilePath));
}