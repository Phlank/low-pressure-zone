namespace LowPressureZone.Core.Domain;

public static class Rule
{
    public static List<RuleError> Apply(params List<IRule> rules)
    {
        List<RuleError> errors = [];
        foreach (var rule in rules)
        {
            if (rule.IsBroken()) errors.Add(rule.Error);
        }

        return errors;
    }

    public static DomainResult<T> ApplyIntoResult<T>(T valueIfSuccess, params List<IRule> rules)
    {
        var errors = Apply(rules);
        if (errors.Count == 0)
        {
            return DomainResult.Ok(valueIfSuccess);
        }

        return DomainResult.Err<T>(errors);
    }
}