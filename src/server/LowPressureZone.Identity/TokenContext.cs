using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;

namespace LowPressureZone.Identity;

public sealed class TokenContext
{
    public required string Email { get; set; }
    public required string Token { get; set; }

    [JsonIgnore]
    public string Encoded => Base64UrlEncoder.Encode(JsonSerializer.Serialize(this, JsonSerializerOptions.Web));

    public static TokenContext? Decode(string encodedContext)
    {
        var serialized = Base64UrlEncoder.Decode(encodedContext);
        return JsonSerializer.Deserialize<TokenContext>(serialized, JsonSerializerOptions.Web);
    }
}