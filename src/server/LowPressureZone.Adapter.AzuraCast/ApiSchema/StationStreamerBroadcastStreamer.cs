using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public sealed class StationStreamerBroadcastStreamer
{
    public int Id { get; set; }

    [JsonPropertyName("streamer_username")]
    public string StreamerUsername { get; set; } = string.Empty;

    [JsonPropertyName("display_name")] public string DisplayName { get; set; } = string.Empty;
}