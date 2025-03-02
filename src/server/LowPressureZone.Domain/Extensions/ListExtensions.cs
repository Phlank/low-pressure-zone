using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowPressureZone.Domain.Extensions;

public static class ListExtensions
{
    public static void AddIfNotNull<T>(this IList<T> list, T? item) where T : class
    {
        if (item == null) return;
        list.Add(item);
    }
}
