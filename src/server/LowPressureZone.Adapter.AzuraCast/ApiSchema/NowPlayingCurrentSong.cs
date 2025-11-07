using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public record NowPlayingCurrentSong
{
    [JsonPropertyName("sh_id")] public int ShId { get; set; }

    [JsonPropertyName("played_at")] public int PlayedAt { get; set; }

    public int Duration { get; set; }

    public string? Playlist { get; set; }
    public string? Streamer { get; set; }

    [JsonPropertyName("is_request")] public bool IsRequest { get; set; }

    public required Song Song { get; set; }

    public int Elapsed { get; set; }
    public int Remaining { get; set; }
}