using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public class StationMediaRequest
{
    public string? Title { get; set; }
    public string? Album { get; set; }
    public string? Artist { get; set; }
    public string? Genre { get; set; }
    public string? Isrc { get; set; }
    public string? Lyrics { get; set; }
    public required string Path { get; set; }
    public IEnumerable<int> Playlists { get; set; } = [];

    [JsonPropertyName("extra_metadata")]
    public StationMediaExtraMetadata ExtraMetadata { get; set; } = new();
}