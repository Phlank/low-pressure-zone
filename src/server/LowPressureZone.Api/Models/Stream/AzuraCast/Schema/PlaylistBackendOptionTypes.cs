using System.Text.Json.Serialization;

namespace LowPressureZone.Api.Models.Stream.AzuraCast.Schema;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PlaylistBackendOptionTypes
{
    [JsonStringEnumMemberName("interrupt")]
    Interrupt,

    [JsonStringEnumMemberName("loop_once")]
    LoopOnce,

    [JsonStringEnumMemberName("single_track")]
    SingleTrack,
    [JsonStringEnumMemberName("merge")] Merge
}
