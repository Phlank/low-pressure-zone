namespace LowPressureZone.Api.Extensions;

public static class DateTimeOffsetExtensions
{
    public static int ToIso8601TimeInt(this DateTimeOffset self)
        => self.Hour * 100 + self.Minute;
}