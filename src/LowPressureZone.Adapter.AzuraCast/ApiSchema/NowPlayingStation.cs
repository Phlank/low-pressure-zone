using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public record NowPlayingStation
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Shortcode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Frontend { get; set; } = string.Empty;
    public string Backend { get; set; } = string.Empty;
    public string Timezone { get; set; } = string.Empty;

    [JsonPropertyName("listen_url")]
    public string ListenUrl { get; set; } = string.Empty;

    public string? Url { get; set; }

    [JsonPropertyName("public_player_url")]
    public string PublicPlayerUrl { get; set; } = string.Empty;

    [JsonPropertyName("playlist_pls_url")]
    public string PlaylistPlsUrl { get; set; } = string.Empty;

    [JsonPropertyName("playlist_m3u_url")]
    public string PlaylistM3uUrl { get; set; } = string.Empty;

    [JsonPropertyName("is_public")]
    public bool? IsPublic { get; set; }

    public NowPlayingStationMount[] Mounts { get; set; } = [];
    public NowPlayingStationRemote[] Remotes { get; set; } = [];

    [JsonPropertyName("hls_is_default")]
    public bool? HlsIsDefault { get; set; }

    [JsonPropertyName("hls_url")]
    public string? HlsUrl { get; set; }

    [JsonPropertyName("hls_listeners")]
    public int HlsListeners { get; set; }
}