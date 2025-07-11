using System.Text.Json.Serialization;

namespace LowPressureZone.Api.Models.Stream.AzuraCast.Schema;

public class BroadcastStreamer
{
    public int Id { get; set; }

    [JsonPropertyName("streamer_username")]
    public string StreamerUsername { get; set; } = string.Empty;

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = string.Empty;
}
