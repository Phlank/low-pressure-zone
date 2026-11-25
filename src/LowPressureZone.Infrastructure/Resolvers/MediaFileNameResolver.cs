using System.Globalization;
using System.Text;

namespace LowPressureZone.Infrastructure.Resolvers;

public class MediaFileNameResolver
{
    public static string Resolve(DateTimeOffset startsAt, string artist, string? title)
    {
        var nameBuilder = new StringBuilder(artist);
        if (!string.IsNullOrWhiteSpace(title))
        {
            nameBuilder.Append("_").Append(title);
        }
        nameBuilder.Append("_").Append(startsAt.ToString("yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture));
        return Paths.CleanFileName(nameBuilder.ToString());
    }
}