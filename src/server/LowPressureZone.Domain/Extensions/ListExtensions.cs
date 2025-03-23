namespace LowPressureZone.Domain.Extensions;

public static class ListExtensions
{
    public static void AddIfNotNull<T>(this IList<T> list, T? item) where T : class
    {
        if (item == null) return;
        list.Add(item);
    }
}
