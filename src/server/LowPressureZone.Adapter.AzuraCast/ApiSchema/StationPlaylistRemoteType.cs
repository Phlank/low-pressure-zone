using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StationPlaylistRemoteType
{
    [JsonStringEnumMemberName("stream")]
    Stream,

    [JsonStringEnumMemberName("playlist")]
    Playlist,

    [JsonStringEnumMemberName("other")]
    Other
}