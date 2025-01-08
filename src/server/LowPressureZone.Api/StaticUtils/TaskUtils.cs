namespace LowPressureZone.Api.StaticUtils;

public static class TaskUtils
{
    public static Task DelayFromTime(DateTime start, int milliseconds)
    {
        var elapsed = DateTime.Now - start;
        var remainingMilliseconds = Math.Clamp(milliseconds - elapsed.TotalMilliseconds, 0, milliseconds);
        return Task.Delay((int)remainingMilliseconds);
    }
}
