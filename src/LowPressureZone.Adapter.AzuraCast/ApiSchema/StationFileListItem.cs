using System.Text.Json.Serialization;
using LowPressureZone.Core.JsonConverters;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public class StationFileListItem
{
    public Dictionary<string, string> Links { get; set; } = new();
    public required string Path { get; set; }

    [JsonPropertyName("path_short")]
    public required string PathShort { get; set; }

    public required string Text { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FileType Type { get; set; }

    [JsonConverter(typeof(UnixTimestampDateTimeOffsetConverter))]
    public DateTimeOffset Timestamp { get; set; }

    public int? Size { get; set; }
    public StationMedia? Media { get; set; }

    [JsonPropertyName("dir")]
    public FileListDirectory? Directory { get; set; }
}