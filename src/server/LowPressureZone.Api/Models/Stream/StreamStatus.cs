namespace LowPressureZone.Api.Models.Stream;

public sealed class StreamStatus
{
    public required bool IsOnline { get; set; }
    public required bool IsLive { get; set; }
    public required bool IsStatusStale { get; set; }
    public required string? Name { get; set; }
    public required string? Type { get; set; }
    public required string? ListenUrl { get; set; }
    public required int ListenerCount { get; set; }
    public required DateTimeOffset? StartedAt { get; set; }
    public required TimeSpan? Duration { get; set; }
}