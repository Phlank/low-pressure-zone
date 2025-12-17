using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StationPlaylistType
{
    [JsonStringEnumMemberName("default")]
    Default,
    [JsonStringEnumMemberName("once_per_x_songs")]
    OncePerXSongs,
    [JsonStringEnumMemberName("once_per_x_minutes")]
    OncePerXMinutes,
    [JsonStringEnumMemberName("once_per_hour")]
    OncePerHour,
    [JsonStringEnumMemberName("custom")]
    Custom
}