using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public enum StationPlaylistBackendOption
{
    [JsonStringEnumMemberName("interrupt")]
    Interrupt,
    [JsonStringEnumMemberName("single_track")]
    SingleTrack,
    [JsonStringEnumMemberName("merge")]
    Merge
}