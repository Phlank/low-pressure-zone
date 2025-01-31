using LowPressureZone.Domain.Interfaces;

namespace LowPressureZone.Api.Extensions;

public static class DateTimeRangeExtensions
{
    public static bool IsWithin(this IDateTimeRange self, IDateTimeRange other)
    {
        return self.Start >= other.Start && self.End <= other.End;
    }
}
