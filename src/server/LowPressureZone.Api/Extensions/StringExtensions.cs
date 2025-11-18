namespace LowPressureZone.Api.Extensions;

public static class StringExtensions
{
    public static bool StartsWithAny(
        this string value,
        IEnumerable<string> itemsToCompare,
        StringComparison comparison = StringComparison.InvariantCulture) =>
        itemsToCompare.Any((item) => value.StartsWith(item, comparison));
}