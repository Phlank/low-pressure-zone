using System.Text.Json.Serialization;

namespace LowPressureZone.Api.Models.Stream.AzuraCast.Schema;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PlaylistTypes
{
    [JsonPropertyName("default")] Default,
    [JsonPropertyName("once_per_x_songs")] OncePerXSongs,

    [JsonPropertyName("once_per_x_minutes")]
    OncePerXMinutes,
    [JsonPropertyName("once_per_hour")] OncePerHour,
    [JsonPropertyName("custom")] Custom
}
