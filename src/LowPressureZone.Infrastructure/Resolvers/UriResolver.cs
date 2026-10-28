using FastEndpoints;
using LowPressureZone.Infrastructure.Configuration;
using LowPressureZone.Infrastructure.Identity;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Infrastructure.Resolvers;

[RegisterService<UriResolver>(LifeTime.Singleton)]
public sealed class UriResolver(IOptions<UrlConfiguration> options)
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