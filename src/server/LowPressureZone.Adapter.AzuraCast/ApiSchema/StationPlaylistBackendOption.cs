using LowPressureZone.Core.JsonConverters;
using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

[JsonConverter(typeof(JsonEnumStringConverterWithEmptyStringToNoneConverter<StationPlaylistBackendOption>))]
public enum StationPlaylistBackendOption
{
    None,

    [JsonStringEnumMemberName("interrupt")]
    Interrupt,

    [JsonStringEnumMemberName("single_track")]
    SingleTrack,

    [JsonStringEnumMemberName("merge")]
    Merge
}