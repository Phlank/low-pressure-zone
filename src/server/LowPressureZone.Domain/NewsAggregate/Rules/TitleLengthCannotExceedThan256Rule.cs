using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.NewsAggregate.Rules;

public class TitleLengthCannotExceedThan256Rule(string title) : IRule
{
    public bool IsBroken() => title.Trim().Length > 256;

    public RuleError Error => new("Cannot be more than 256 characters", nameof(News.Title));
}