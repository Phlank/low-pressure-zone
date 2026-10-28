using FastEndpoints;
using LowPressureZone.Identity;
using LowPressureZone.Infrastructure.Configuration;
using LowPressureZone.Infrastructure.Identity;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Api.Services;

[RegisterService<UriService>(LifeTime.Singleton)]
public sealed class UriService(IOptions<UrlConfiguration> options)
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