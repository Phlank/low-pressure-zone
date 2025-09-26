using System.Text.Json.Serialization;

namespace LowPressureZone.Api.Models.Stream.AzuraCast.Schema;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PlaylistRemoteTypes
{
    [JsonPropertyName("stream")] Stream,
    [JsonPropertyName("playlist")] Playlist,
    [JsonPropertyName("other")] Other
}
