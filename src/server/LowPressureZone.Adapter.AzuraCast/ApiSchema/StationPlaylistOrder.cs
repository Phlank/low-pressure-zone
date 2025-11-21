using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public enum StationPlaylistOrder
{
    [JsonStringEnumMemberName("random")]
    Random,
    [JsonStringEnumMemberName("shuffle")]
    Shuffle,
    [JsonStringEnumMemberName("sequential")]
    Sequential
}