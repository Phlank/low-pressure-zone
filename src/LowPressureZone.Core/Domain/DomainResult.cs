using FluentValidation.Results;
using LowPressureZone.Core.Extensions;

namespace LowPressureZone.Core.Domain;

public class DomainResult<T> : Result<T, List<RuleError>>
{
    public List<IEvent> Events { get; private init; } = [];

    internal DomainResult(T? value, List<IEvent>? events, List<RuleError>? error) : base(value, error)
    {
        if (events is not null)
            Events = events;
    }

    public List<RuleError> Errors => IsError ? Error : [];

    public DomainResult<NoValue> ToNoValue() =>
        IsError ? DomainResult.Err<NoValue>(Errors) : DomainResult.Ok();


    public List<ValidationFailure> Failures =>
        [.. Errors.Select(error => new ValidationFailure(error.Field, error.Message))];
}

public static class DomainResult
{
    public static DomainResult<NoValue> Ok() => new(NoValue.Instance, null, null);
    public static DomainResult<NoValue> Ok(List<IEvent> events) => new(NoValue.Instance, events, null);
    public static DomainResult<T> Ok<T>(T value) => new(value, null, null);
    public static DomainResult<T> Ok<T>(T value, List<IEvent> events) => new(value, events, null);
    public static DomainResult<NoValue> Err(List<RuleError> errors) => new(default, null, errors);
    public static DomainResult<T> Err<T>(RuleError error) => new(default, null, [error]);
    public static DomainResult<T> Err<T>(List<RuleError> errors) => new(default, null, errors);

    public static DomainResult<NoValue> Compose(params List<DomainResult<NoValue>> results) =>
        new(NoValue.Instance,
            [.. results.SelectMany(r => r.Events)],
            [.. results.SelectMany(r => r.Errors)]);

    public static DomainResult<T> WithAdditionalErrors<T>(this DomainResult<T> result, params List<RuleError> errors) =>
        new DomainResult<T>(result.IsError || !errors.IsEmpty ? default : result.Value,
                            result.Events,
                            result.IsError || !errors.IsEmpty ? null : [.. result.Errors, .. errors]);

    public static DomainResult<T> WithAdditionalRules<T>(this DomainResult<T> result, params List<IRule> rules) =>
        result.WithAdditionalErrors(Rule.Apply(rules));
}