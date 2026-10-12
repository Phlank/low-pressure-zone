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

    public static DomainResult<NoValue> ApplyIntoResult(params List<IRule> rules) =>
        ApplyIntoResult(NoValue.Instance, [], rules);

    public static DomainResult<T> ApplyIntoResult<T>(T valueIfSuccess, params List<IRule> rules) =>
        ApplyIntoResult(valueIfSuccess, [], rules);

    public static DomainResult<T> ApplyIntoResult<T>(T valueIfSuccess,
                                                     IEvent eventIfSuccess,
                                                     params List<IRule> rules) =>
        ApplyIntoResult(valueIfSuccess, [eventIfSuccess], rules);

    public static DomainResult<T> ApplyIntoResult<T>(T valueIfSuccess,
                                                     List<IEvent> eventsIfSuccess,
                                                     params List<IRule> rules)
    {
        var errors = Apply(rules);
        if (errors.Count == 0)
        {
            return DomainResult.Ok(valueIfSuccess, eventsIfSuccess);
        }

        return DomainResult.Err<T>(errors);
    }
}