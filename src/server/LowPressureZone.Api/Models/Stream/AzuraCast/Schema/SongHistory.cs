using System.Text.Json.Serialization;

namespace LowPressureZone.Api.Models.Stream.AzuraCast.Schema;

public record SongHistory
{
    [JsonPropertyName("sh_id")]
    public required string ShId { get; set; }

    [JsonPropertyName("played_at")]
    public int PlayedAt { get; set; }

    public int Duration { get; set; }
    public string? Playlist { get; set; }
    public string? Streamer { get; set; }

    [JsonPropertyName("is_request")]
    public bool IsRequest { get; set; }

    public required Song Song { get; set; }
}
