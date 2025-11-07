namespace LowPressureZone.Core;

public sealed class Result<T, TErr>(T? value, TErr? error)
{
    public T Value =>
        value ?? throw new InvalidOperationException("Cannot access the value of an unsuccessful result");

    public TErr Error =>
        error ?? throw new InvalidOperationException("Cannot access the error of a successful result");

    public bool IsSuccess => error is null;
}

public static class Result
{
    public static Result<T, TErr> Ok<T, TErr>(T data) => new(data, default);
    public static Result<T, TErr> Err<T, TErr>(TErr error) => new(default, error);
}
