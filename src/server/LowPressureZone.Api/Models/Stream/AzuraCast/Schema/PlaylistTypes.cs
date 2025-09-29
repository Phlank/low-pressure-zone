using System.Text.Json.Serialization;

namespace LowPressureZone.Api.Models.Stream.AzuraCast.Schema;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PlaylistTypes
{
    [JsonStringEnumMemberName("default")] Default,

    [JsonStringEnumMemberName("once_per_x_songs")]
    OncePerXSongs,

    [JsonStringEnumMemberName("once_per_x_minutes")]
    OncePerXMinutes,

    [JsonStringEnumMemberName("once_per_hour")]
    OncePerHour,
    [JsonStringEnumMemberName("custom")] Custom
}
