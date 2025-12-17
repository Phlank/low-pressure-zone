using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public class StationMediaPlaylist
{
    public int Id { get; set; }
    public required string Name { get; set; }

    [JsonPropertyName("short_name")]
    public required string ShortName { get; set; }

    public string? Folder { get; set; }
    public int Count { get; set; }
}