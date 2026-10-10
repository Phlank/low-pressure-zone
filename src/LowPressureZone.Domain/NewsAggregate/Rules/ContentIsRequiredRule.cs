using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.NewsAggregate.Rules;

public class ContentIsRequiredRule(string content) : IRule
{
    public bool IsBroken() => string.IsNullOrWhiteSpace(content);

    public RuleError Error => new("Required", nameof(News.Content));
}