using System.Text.Json.Serialization;

namespace LowPressureZone.Api.Models.Stream.AzuraCast.Schema;

public class StationPlaylist
{
    public required string Name { get; set; }
    public required PlaylistTypes Types { get; set; }
    public required PlaylistSources Source { get; set; }
    public required PlaylistOrders Order { get; set; }

    [JsonPropertyName("remote_url")] public string? RemoteUrl { get; set; }

    [JsonPropertyName("remote_type")] public PlaylistRemoteTypes? RemoteType { get; set; }

    [JsonPropertyName("remote_buffer")] public int RemoteBuffer { get; set; }

    [JsonPropertyName("is_enabled")] public bool IsEnabled { get; set; }

    [JsonPropertyName("is_jingle")] public bool IsJingle { get; set; }

    [JsonPropertyName("play_per_songs")] public int PlayPerSongs { get; set; }

    [JsonPropertyName("play_per_minutes")] public int PlayPerMinutes { get; set; }

    [JsonPropertyName("play_per_hour_minute")]
    public int PlayPerHourMinute { get; set; }

    public int Weight { get; set; }

    [JsonPropertyName("include_in_requests")]
    public bool IncludeInRequests { get; set; }

    [JsonPropertyName("include_in_on_demand")]
    public bool IncludeInOnDemand { get; set; }

    [JsonPropertyName("backend_options")]
    public ICollection<PlaylistBackendOptionTypes> BackendOptions { get; set; } = [];

    [JsonPropertyName("avoid_duplicates")] public bool AvoidDuplicates { get; set; }

    [JsonPropertyName("schedule_items")] public StationSchedule? ScheduleItems { get; set; }
    // Omitting the Podcast collection for now
}
