namespace LowPressureZone.Api.Endpoints.Stream.Status;

public sealed class StreamStatusResponse
{
    public required bool IsOnline { get; set; }
    public required bool IsLive { get; set; }
    public required bool IsStatusStale { get; set; }
    public required string? Name { get; set; }
    public required string? Type { get; set; }
    public required string? ListenUrl { get; set; }
    public required int ListenerCount { get; set; }
    public required DateTimeOffset? StartedAt { get; set; }
    public required double? DurationSeconds { get; set; }
}