namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public class StationPlaylist
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; } = "";
    public required StationPlaylistType Type { get; set; }
    public required StationPlaylistSource Source { get; set; }
    public required StationPlaylistOrder Order { get; set; }
    public string? RemoteUrl { get; set; }
    public StationPlaylistRemoteType RemoteType { get; set; }
    public int RemoteBuffer { get; set; }
    public required bool IsEnabled { get; set; }
    public bool IsJingle { get; set; }
    public int PlayPerSongs { get; set; }
    public int PlayPerMinutes { get; set; }
    public int PlayPerHourMinute { get; set; }
    public required int Weight { get; set; }
    public bool IncludeInRequests { get; set; }
    public bool IncludeInOnDemand { get; set; }
    public IEnumerable<StationPlaylistBackendOption> BackendOptions { get; set; } = [];
    public bool AvoidDuplicates { get; set; }
    public IEnumerable<StationPlaylistScheduleItem> ScheduleItems { get; set; } = [];
}