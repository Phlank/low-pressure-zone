namespace LowPressureZone.Api.Extensions;

public static class DateTimeOffsetExtensions
{
    public static int ToIso8601TimeInt(this DateTimeOffset self)
        => self.Hour * 100 + self.Minute;

    public static DateTimeOffset TopOfHour(this DateTimeOffset time, int hoursToAdd = 0) =>
        new DateTimeOffset(time.Year, time.Month, time.Day, time.Hour, 0, 0, time.Offset).AddHours(hoursToAdd);
}