using System.Text.Json;
using LowPressureZone.Api.Endpoints.Users.Register;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LowPressureZone.Api.Services;

public class UriService(IOptions<UriServiceOptions> options)
{
    public Uri GetRegisterUri(string email, string token)
    {
        var context = new RegistrationContext
        {
            Email = email,
            Token = token
        };
        var builder = new UriBuilder(options.Value.RegisterUrl)
        {
            Query = $"?context={context.Encoded}"
        };
        return builder.Uri;
    }
}
