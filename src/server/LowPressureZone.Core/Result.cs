using FluentValidation.Results;

namespace LowPressureZone.Core;

public sealed class Result<T, TErr>(T? value, TErr? error) where TErr : notnull
{
    public T Value =>
        value ?? throw new InvalidOperationException("Cannot access the value of an unsuccessful result");

    public TErr Error =>
        error ?? throw new InvalidOperationException("Cannot access the error of a successful result");

    public bool IsSuccess => error is null;
    public bool IsError => error is not null;
}

public static class Result
{
    public static Result<T, string> Ok<T>(T value) => new(value, null);
    public static Result<T, TErr> Ok<T, TErr>(T data) where TErr : notnull => new(data, default);
    
    public static Result<T, string> Err<T>(string error) => new(default, error);

    public static Result<T, IEnumerable<ValidationFailure>> Err<T>(IEnumerable<ValidationFailure> errors) =>
        new(default, errors);
    public static Result<T, TErr> Err<T, TErr>(TErr error) where TErr : notnull => new(default, error);
}