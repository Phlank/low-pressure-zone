using LowPressureZone.Api.Models.Options;
using LowPressureZone.Identity;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Api.Services;

public class UriService(IOptions<UrlOptions> options)
{
    public Uri GetInviteUrl(TokenContext context)
    {
        var builder = new UriBuilder(options.Value.RegisterUrl)
        {
            Query = $"?context={context.Encoded}"
        };
        return builder.Uri;
    }

    public Uri GetResetPasswordUrl(TokenContext context)
    {
        var builder = new UriBuilder(options.Value.ResetPasswordUrl)
        {
            Query = $"?context={context.Encoded}"
        };
        return builder.Uri;
    }
}
