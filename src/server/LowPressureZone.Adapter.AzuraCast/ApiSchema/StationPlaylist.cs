using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public sealed class StationPlaylist
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public string Description { get; init; } = "";
    public required StationPlaylistType Type { get; init; }
    public required StationPlaylistSource Source { get; init; }
    public required StationPlaylistOrder Order { get; init; }
    [JsonPropertyName("remote_url")]
    public string? RemoteUrl { get; init; }
    [JsonPropertyName("remote_type")]
    public StationPlaylistRemoteType RemoteType { get; init; }
    [JsonPropertyName("remote_buffer")]
    public int RemoteBuffer { get; init; }
    [JsonPropertyName("is_enabled")]
    public bool IsEnabled { get; init; } = true;
    [JsonPropertyName("is_jingle")]
    public bool IsJingle { get; init; }
    [JsonPropertyName("play_per_songs")]
    public int PlayPerSongs { get; init; }
    [JsonPropertyName("play_per_minutes")]
    public int PlayPerMinutes { get; init; }
    [JsonPropertyName("play_per_hour_minute")]
    public int PlayPerHourMinute { get; init; }
    public required int Weight { get; init; }
    [JsonPropertyName("include_in_requests")]
    public bool IncludeInRequests { get; init; }
    [JsonPropertyName("include_in_on_demand")]
    public bool IncludeInOnDemand { get; init; }
    [JsonPropertyName("backend_options")]
    public IEnumerable<StationPlaylistBackendOption> BackendOptions { get; init; } = [];
    [JsonPropertyName("avoid_duplicates")]
    public bool AvoidDuplicates { get; init; }
    [JsonPropertyName("schedule_items")]
    public IEnumerable<StationPlaylistScheduleItem> ScheduleItems { get; init; } = [];
}