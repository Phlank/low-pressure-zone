using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public sealed record NowPlaying
{
    public required NowPlayingStation Station { get; set; }
    public required NowPlayingListeners Listeners { get; set; }
    public required NowPlayingLive Live { get; set; }

    [JsonPropertyName("now_playing")]
    public NowPlayingCurrentSong? CurrentSong { get; set; }

    [JsonPropertyName("playing_next")]
    public NowPlayingStationQueue? PlayingNext { get; set; }

    [JsonPropertyName("song_history")]
    public NowPlayingSongHistory[] SongHistory { get; set; } = [];

    [JsonPropertyName("is_online")]
    public bool IsOnline { get; set; }

    public string? Cache { get; set; }
}