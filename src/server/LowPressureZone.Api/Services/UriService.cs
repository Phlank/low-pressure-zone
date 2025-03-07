using LowPressureZone.Identity;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Api.Services;

public class UriService(IOptions<UriServiceOptions> options)
{
    public Uri GetInviteUrl(TokenContext context)
    {
        var builder = new UriBuilder(options.Value.RegisterUrl)
        {
            Query = $"?context={context.Encoded}"
        };
        return builder.Uri;
    }
}
