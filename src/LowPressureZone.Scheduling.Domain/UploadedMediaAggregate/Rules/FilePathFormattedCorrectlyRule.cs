using System.Text.RegularExpressions;
using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.UploadedMediaAggregate.Rules;

public class FilePathFormattedCorrectlyRule(UploadedMedia value) : IRule
{
    public bool IsBroken() => !new Regex($"^[A-Za-z0-9]+/{Regex.Escape(value.FileName)}$").IsMatch(value.FilePath);

    public RuleError Error => new("Must be formatted as 'folder/fileName'", nameof(UploadedMedia.FilePath));
}