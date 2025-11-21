using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public enum StationPlaylistSource
{
    [JsonStringEnumMemberName("songs")]
    Songs,
    [JsonStringEnumMemberName("remote_url")]
    RemoteUrl
}