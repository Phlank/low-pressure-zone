namespace LowPressureZone.Api.Models;

public class Result<T, TErr>
{
    private T? _data;
    private TErr? _error;

    public Result(T data)
    {
        _data = data;
    }

    public Result(TErr error)
    {
        _error = error;
    }
    public T? Data => _data;
    public TErr? Error => _error;
    public bool IsSuccess => Error is null;
}
