namespace LowPressureZone.Core.Domain;

public static class Rule
{
    public static List<RuleError> Apply(params List<IRule> rules)
    {
        List<RuleError> errors = [];
        var brokenRules = rules.Where(rule => rule.IsBroken());
        foreach (var rule in brokenRules)
        {
            errors.Add(rule.Error);
        }

        return errors;
    }

    public static DomainResult<NoValue> ApplyIntoEmptyResult(params List<IRule> rules) =>
        ApplyIntoResult(NoValue.Instance, [], rules);

    public static DomainResult<T> ApplyIntoResult<T>(T valueIfSuccess, params List<IRule> rules) =>
        ApplyIntoResult(valueIfSuccess, [], rules);

    public static DomainResult<NoValue> ApplyIntoEmptyResult(IEvent eventIfSuccess, params List<IRule> rules) =>
        ApplyIntoResult(NoValue.Instance, [eventIfSuccess], rules);

    public static DomainResult<T> ApplyIntoResult<T>(T valueIfSuccess,
                                                     IEvent eventIfSuccess,
                                                     params List<IRule> rules) =>
        ApplyIntoResult(valueIfSuccess, [eventIfSuccess], rules);
    
    public static DomainResult<NoValue> ApplyIntoEmptyResult(List<IEvent> events, params List<IRule> rules) =>
        ApplyIntoResult(NoValue.Instance, events, rules);

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