using System.Text.Json.Serialization;

namespace LowPressureZone.Api.Models.Stream.AzuraCast.Schema;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PlaylistRemoteTypes
{
    [JsonStringEnumMemberName("stream")] Stream,
    [JsonStringEnumMemberName("playlist")] Playlist,
    [JsonStringEnumMemberName("other")] Other
}
