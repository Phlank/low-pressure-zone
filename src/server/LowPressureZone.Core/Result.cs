using FluentValidation.Results;

namespace LowPressureZone.Core;

public class Result<T, TErr>(T? value, TErr? error) where TErr : notnull
{
    public T Value =>
        value ?? throw new InvalidOperationException("Cannot access the value of an unsuccessful result");

    public TErr Error =>
        error ?? throw new InvalidOperationException("Cannot access the error of a successful result");

    public bool IsSuccess => error is null;
    public bool IsError => error is not null;
}

public sealed class Result<TErr>(TErr? error = default) : Result<bool?, TErr>(null, error) where TErr : notnull;

public static class Result
{
    public static Result<T, string> Ok<T>(T value) => new(value, null);

    public static Result<T, TErr> Ok<T, TErr>(T value) where TErr : notnull => new(value, default);

    public static Result<T, string> Err<T>(string error) => new(default, error);

    public static Result<T, ValidationFailure> Err<T>(ValidationFailure error) => new(default, error);

    public static Result<T, IEnumerable<ValidationFailure>> Err<T>(IEnumerable<ValidationFailure> errors) =>
        new(default, errors);

    public static Result<T, TErr> Err<T, TErr>(TErr error) where TErr : notnull => new(default, error);

    public static Result<TErr> Err<TErr>(TErr error) where TErr : notnull => new(error);
}