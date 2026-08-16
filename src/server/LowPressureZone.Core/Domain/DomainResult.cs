using FluentValidation.Results;

namespace LowPressureZone.Core.Domain;

public class DomainResult<T> : Result<T, List<RuleError>>
{
    internal DomainResult(T? value, List<RuleError>? error) : base(value, error)
    {
    }

    public List<RuleError> Errors => IsError ? Error : [];

    public DomainResult<NoValue> ToNoValue() =>
        IsError ? DomainResult.Err<NoValue>(Errors) : DomainResult.Ok();

    public List<ValidationFailure> Failures =>
        [.. Errors.Select(error => new ValidationFailure(error.Field, error.Message))];
}

public static class DomainResult
{
    public static DomainResult<NoValue> Ok() => new(NoValue.Instance, null);
    public static DomainResult<T> Ok<T>(T value) => new(value, null);
    public static DomainResult<NoValue> Err(List<RuleError> errors) => new(default, errors);
    public static DomainResult<T> Err<T>(RuleError error) => new(default, [error]);
    public static DomainResult<T> Err<T>(List<RuleError> errors) => new(default, errors);

    public static DomainResult<NoValue> Compose(params List<DomainResult<NoValue>> results) =>
        new(default, [.. results.SelectMany(r => r.Errors)]);
}