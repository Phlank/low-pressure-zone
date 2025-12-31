namespace LowPressureZone.Api.Utilities;

public static class DateTimeUtilities
{
    public static DateTime GetNextHour(int hour)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(hour, 23, nameof(hour));
        var now = DateTime.UtcNow;
        if (now.Hour < hour)
            return new DateTime(now.Year, now.Month, now.Day, hour, 0, 0);
        return new DateTime(now.Year, now.Month, now.Day, hour, 0, 0).AddDays(1);
    }
}