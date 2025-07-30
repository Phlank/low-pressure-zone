using System.Text.Json.Serialization;
using LowPressureZone.Api.Models.Stream.AzuraCast.Schema;

namespace LowPressureZone.Api.Models.Stream.AzuraCast;

public record NowPlayingResponse
{
    public required Station Station { get; set; }
    public required Listeners Listeners { get; set; }
    public required LiveInfo Live { get; set; }

    [JsonPropertyName("now_playing")] public CurrentSong? NowPlaying { get; set; }

    [JsonPropertyName("playing_next")] public StationQueue? PlayingNext { get; set; }

    [JsonPropertyName("song_history")] public SongHistory[] SongHistory { get; set; } = [];

    [JsonPropertyName("is_online")] public bool IsOnline { get; set; }

    public string[] Cache { get; set; } = [];
}