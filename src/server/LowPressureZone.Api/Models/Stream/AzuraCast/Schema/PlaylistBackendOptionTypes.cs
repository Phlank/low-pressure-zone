using System.Text.Json.Serialization;

namespace LowPressureZone.Api.Models.Stream.AzuraCast.Schema;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PlaylistBackendOptionTypes
{
    [JsonPropertyName("interrupt")] Interrupt,
    [JsonPropertyName("loop_once")] LoopOnce,
    [JsonPropertyName("single_track")] SingleTrack,
    [JsonPropertyName("merge")] Merge
}
