namespace LowPressureZone.Api.Utilities;

internal static class TaskUtilities
{
    private const int ResponseDelayMs = 3000;
    public static Task DelaySensitiveResponse(DateTime requestTime)
    {
        var elapsed = DateTime.UtcNow - requestTime;
        if (elapsed.Ticks < 0) return Task.CompletedTask;
        var wait = new TimeSpan(0, 0, 0, 0, (int)Math.Clamp(ResponseDelayMs - elapsed.TotalMilliseconds, 1, ResponseDelayMs));
        return Task.Delay(wait);
    }
}
