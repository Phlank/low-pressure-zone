using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public enum StationPlaylistType
{
    [JsonStringEnumMemberName("default")]
    Default,
    [JsonStringEnumMemberName("once_per_x_songs")]
    OncePerXSongs,
    [JsonStringEnumMemberName("once_per_x_hours")]
    OncePerXHours,
    [JsonStringEnumMemberName("once_per_hour")]
    OncePerHour,
    [JsonStringEnumMemberName("custom")]
    Custom
}