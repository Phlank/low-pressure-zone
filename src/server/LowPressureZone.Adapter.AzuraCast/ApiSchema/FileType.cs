using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public enum FileType
{
    [JsonStringEnumMemberName("directory")]
    Directory,
    [JsonStringEnumMemberName("media")]
    Media,
    [JsonStringEnumMemberName("cover_art")]
    CoverArt,
    [JsonStringEnumMemberName("unprocessable_file")]
    UnprocessableFile,
    [JsonStringEnumMemberName("other")]
    Other
}