using System.Text.Json;
using LowPressureZone.Api.Endpoints.Users.Register;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LowPressureZone.Api.Services;

public class UriService
{
    private readonly string _registerUrl;
    
    public UriService(IOptions<UriServiceOptions> options)
    {
        _registerUrl = options.Value.RegisterUrl;
    }

    public Uri GetRegisterUri(string email, string token)
    {
        var context = new RegistrationContext
        {
            Email = email,
            Token = token
        };
        var builder = new UriBuilder(_registerUrl);
        builder.Query = $"?context={context.Encoded}";
        return builder.Uri;
    }
}
