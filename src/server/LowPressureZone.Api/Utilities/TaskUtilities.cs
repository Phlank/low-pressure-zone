namespace LowPressureZone.Api.Utilities;

public static class TaskUtilities
{
    private const int RESPONSE_DELAY_MS = 3000;
    public static Task DelaySensitiveResponse(DateTime requestTime)
    {
        var elapsed = DateTime.UtcNow - requestTime;
        if (elapsed.Ticks < 0) return Task.CompletedTask;
        var wait = new TimeSpan(0, 0, 0, 0, (int)Math.Clamp(RESPONSE_DELAY_MS - elapsed.TotalMilliseconds, 1, RESPONSE_DELAY_MS));
        return Task.Delay(wait);
    }
}
