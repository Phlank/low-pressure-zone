namespace LowPressureZone.Api.Models;

public class Result<T, TErr>
{
    private T? _data;
    private TErr? _error;

    private Result(T? data, TErr? error)
    {
        _data = data;
        _error = error;
    }

    public T? Data => _data;
    public TErr? Error => _error;
    public bool IsSuccess => Error is null;

    public static Result<T, TErr> Ok(T data) => new(data, default);
    public static Result<T, TErr> Err(TErr error) => new(default, error);
}
