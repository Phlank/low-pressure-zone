using System.Text.Json.Serialization;
using LowPressureZone.Core.JsonConverters;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public class StationMedia
{
    public int Id { get; set; }

    [JsonPropertyName("unique_id")]
    public string UniqueId { get; set; } = "";

    [JsonPropertyName("song_id")]
    public string SongId { get; set; } = "";

    public string Text { get; set; } = "";
    public string? Artist { get; set; }
    public string? Title { get; set; }
    public string? Album { get; set; }
    public string? Genre { get; set; }
    public string? Isrc { get; set; }
    public string? Lyrics { get; set; }
    public string? Art { get; set; }
    public required string Path { get; set; }

    [JsonConverter(typeof(UnixTimestampDateTimeOffsetConverter))]
    [JsonPropertyName("mtime")]
    public DateTimeOffset ModifiedTime { get; set; }

    [JsonConverter(typeof(UnixTimestampDateTimeOffsetConverter))]
    [JsonPropertyName("uploaded_at")]
    public DateTimeOffset UploadedAt { get; set; }

    [JsonConverter(typeof(UnixTimestampDateTimeOffsetConverter))]
    [JsonPropertyName("art_updated_at")]
    public DateTimeOffset ArtUpdatedAt { get; set; }

    public double Length { get; set; }

    [JsonPropertyName("length_text")]
    public string LengthText { get; set; } = "";

    [JsonPropertyName("extra_metadata")]
    public StationMediaExtraMetadata ExtraMetadata { get; set; } = new();

    public IEnumerable<StationMediaPlaylist> Playlists { get; set; } = [];
}