using LowPressureZone.Domain.Interfaces;

namespace LowPressureZone.Domain.Extensions;

public static class DateTimeRangeExtensions
{
    public static TimeSpan Duration(this IDateTimeRange range) => range.EndsAt - range.StartsAt;
}