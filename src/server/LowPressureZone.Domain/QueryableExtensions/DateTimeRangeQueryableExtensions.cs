using LowPressureZone.Domain.Interfaces;

namespace LowPressureZone.Domain.Extensions;

public static class DateTimeRangeQueryableExtensions
{
    public static IQueryable<TDateTimeRange> WhereOverlaps<TDateTimeRange>(this IQueryable<TDateTimeRange> query, IDateTimeRange range) where TDateTimeRange : class, IDateTimeRange
    {
        return query.Where(q => q.Start < range.End && q.End > range.Start);
    }

    public static IQueryable<TDateTimeRange> WhereInside<TDateTimeRange>(this IQueryable<TDateTimeRange> query, IDateTimeRange range) where TDateTimeRange : class, IDateTimeRange
    {
        return query.Where(q => q.Start >= range.Start && q.End <= range.End);
    }

    public static IQueryable<TDateTimeRange> WhereNotInside<TDateTimeRange>(this IQueryable<TDateTimeRange> query, IDateTimeRange range) where TDateTimeRange: class, IDateTimeRange
    {
        return query.Where(q => q.Start < range.Start || q.End > range.End);
    }

    public static IQueryable<TDateTimeRange> WhereContains<TDateTimeRange>(this IQueryable<TDateTimeRange> query, IDateTimeRange range) where TDateTimeRange : class, IDateTimeRange
    {
        return query.Where(q => q.Start <= range.Start && q.End >= range.End);
    }
}
