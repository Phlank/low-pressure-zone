using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.NewsAggregate.Rules;

public class TitleIsRequiredRule(string title) : IRule
{
    public bool IsBroken() => string.IsNullOrWhiteSpace(title);

    public RuleError Error => new("Required", nameof(News.Title));
}