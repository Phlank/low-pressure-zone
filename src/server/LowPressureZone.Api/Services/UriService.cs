using System.Text.Json;
using LowPressureZone.Api.Endpoints.Users.Register;
using Microsoft.IdentityModel.Tokens;

namespace LowPressureZone.Api.Services;

public class UriService
{
    private readonly UriServiceConfiguration _config;
    
    public UriService(UriServiceConfiguration config)
    {
        _config = config;
    }

    public Uri GetRegisterUri(string email, string token)
    {
        var context = new RegistrationContext
        {
            Email = email,
            Token = token
        };
        var builder = new UriBuilder(_config.RegisterUrl);
        builder.Query = $"?context={context.Encoded}";
        return builder.Uri;
    }
}
