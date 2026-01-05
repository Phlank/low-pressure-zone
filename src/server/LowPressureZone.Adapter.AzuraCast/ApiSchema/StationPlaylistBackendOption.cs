using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StationPlaylistBackendOption
{
    [JsonStringEnumMemberName("interrupt")]
    Interrupt,

    [JsonStringEnumMemberName("single_track")]
    SingleTrack,

    [JsonStringEnumMemberName("merge")]
    Merge
}