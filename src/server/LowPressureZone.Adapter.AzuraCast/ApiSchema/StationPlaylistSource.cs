using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StationPlaylistSource
{
    [JsonStringEnumMemberName("songs")]
    Songs,
    [JsonStringEnumMemberName("remote_url")]
    RemoteUrl
}