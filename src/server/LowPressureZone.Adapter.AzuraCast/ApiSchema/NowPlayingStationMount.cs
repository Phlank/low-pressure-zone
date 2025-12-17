using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public record NowPlayingStationMount : NowPlayingStationRemote
{
    public required string Path { get; set; }

    [JsonPropertyName("is_default")] public required bool IsDefault { get; set; }
}