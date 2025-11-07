using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public record NowPlayingLive
{
    [JsonPropertyName("is_live")]
    public bool IsLive { get; set; }

    [JsonPropertyName("streamer_name")]
    public string StreamerName { get; set; } = string.Empty;

    [JsonPropertyName("broadcast_start")]
    public int? BroadcastStart { get; set; }

    public string? Art { get; set; }
}
