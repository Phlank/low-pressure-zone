using System.Text.Json.Serialization;

namespace LowPressureZone.Api.Models.Stream.AzuraCast.Schema;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PlaylistSources
{
    [JsonPropertyName("songs")] Songs,
    [JsonPropertyName("remote_url")] RemoteUrl
}
