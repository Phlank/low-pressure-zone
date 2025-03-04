using LowPressureZone.Domain.Interfaces;

namespace LowPressureZone.Domain.Extensions;

public static class DateTimeRangeCollectionExtensions
{
    public static IQueryable<TDateTimeRange> WhereOverlaps<TDateTimeRange>(this IQueryable<TDateTimeRange> query, IDateTimeRange range) where TDateTimeRange : class, IDateTimeRange
    {
        return query.Where(q => q.StartsAt < range.EndsAt && q.EndsAt > range.StartsAt);
    }

    public static IEnumerable<TDateTimeRange> WhereOverlaps<TDateTimeRange>(this IEnumerable<TDateTimeRange> query, IDateTimeRange range) where TDateTimeRange : class, IDateTimeRange
    {
        return query.Where(q => q.StartsAt < range.EndsAt && q.EndsAt > range.StartsAt);
    }

    public static IQueryable<TDateTimeRange> WhereInside<TDateTimeRange>(this IQueryable<TDateTimeRange> query, IDateTimeRange range) where TDateTimeRange : class, IDateTimeRange
    {
        return query.Where(q => q.StartsAt >= range.StartsAt && q.EndsAt <= range.EndsAt);
    }

    public static IEnumerable<TDateTimeRange> WhereInside<TDateTimeRange>(this IEnumerable<TDateTimeRange> query, IDateTimeRange range) where TDateTimeRange : class, IDateTimeRange
    {
        return query.Where(q => q.StartsAt >= range.StartsAt && q.EndsAt <= range.EndsAt);
    }

    public static IQueryable<TDateTimeRange> WhereNotInside<TDateTimeRange>(this IQueryable<TDateTimeRange> query, IDateTimeRange range) where TDateTimeRange: class, IDateTimeRange
    {
        return query.Where(q => q.StartsAt < range.StartsAt || q.EndsAt > range.EndsAt);
    }

    public static IEnumerable<TDateTimeRange> WhereNotInside<TDateTimeRange>(this IEnumerable<TDateTimeRange> query, IDateTimeRange range) where TDateTimeRange : class, IDateTimeRange
    {
        return query.Where(q => q.StartsAt < range.StartsAt || q.EndsAt > range.EndsAt);
    }

    public static IQueryable<TDateTimeRange> WhereContains<TDateTimeRange>(this IQueryable<TDateTimeRange> query, IDateTimeRange range) where TDateTimeRange : class, IDateTimeRange
    {
        return query.Where(q => q.StartsAt <= range.StartsAt && q.EndsAt >= range.EndsAt);
    }

    public static IEnumerable<TDateTimeRange> WhereContains<TDateTimeRange>(this IEnumerable<TDateTimeRange> query, IDateTimeRange range) where TDateTimeRange : class, IDateTimeRange
    {
        return query.Where(q => q.StartsAt <= range.StartsAt && q.EndsAt >= range.EndsAt);
    }
}
