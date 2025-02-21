using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;

namespace LowPressureZone.Api.Endpoints.Users.Register;

public class RegistrationContext
{
    public required string Email { get; set; }
    public required string Token { get; set; }

    [JsonIgnore]
    public string Encoded => Base64UrlEncoder.Encode(JsonSerializer.Serialize(this, options: JsonSerializerOptions.Web));

    public static RegistrationContext Decode(string encodedContext)
    {
        var serialized = Base64UrlEncoder.Decode(encodedContext);
        return JsonSerializer.Deserialize<RegistrationContext>(serialized, JsonSerializerOptions.Web) ?? throw new Exception();
    }
}
