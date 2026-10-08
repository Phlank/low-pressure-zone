namespace LowPressureZone.Core.Extensions;

public static class CollectionExtensions
{
    extension<T>(ICollection<T> collection)
    {
        public bool IsEmpty => collection.Count == 0;
    }
}