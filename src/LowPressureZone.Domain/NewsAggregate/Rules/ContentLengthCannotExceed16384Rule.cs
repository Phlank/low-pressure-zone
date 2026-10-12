using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.NewsAggregate.Rules;

public class ContentLengthCannotExceed16384Rule(string content) : IRule
{
    public bool IsBroken() => content.Trim().Length > 16384;

    public RuleError Error => new("Cannot be more than 16384 characters", nameof(News.Content));
}