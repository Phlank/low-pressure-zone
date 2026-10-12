namespace LowPressureZone.Core;

public interface ITimeRange
{
    DateTimeOffset StartsAt { get; }
    DateTimeOffset EndsAt { get; }
}

public static class TimeRangeExtensions
{
    extension(ITimeRange range)
    {
        public bool IsWithin(ITimeRange other) => range.StartsAt >= other.StartsAt
                                                  && range.EndsAt <= other.EndsAt;

        public bool Overlaps(ITimeRange other) => range.StartsAt < other.EndsAt 
                                                  && other.StartsAt < range.EndsAt;
    }
}