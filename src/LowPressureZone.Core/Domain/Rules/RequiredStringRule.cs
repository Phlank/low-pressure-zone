namespace LowPressureZone.Core.Domain.Rules;

public class RequiredStringRule(string fieldName, string value) : IRule
{
    public bool IsBroken() => string.IsNullOrWhiteSpace(value);

    public RuleError Error => new("Required", fieldName);
}