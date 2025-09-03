using FastEndpoints;
using LowPressureZone.Api.Models.Stream;

namespace LowPressureZone.Api.Endpoints.Stream.Status;

public sealed class StreamStatusMapper : IResponseMapper
{
    public StreamStatusResponse FromEntity(StreamStatus status)
        => new()
        {
            IsOnline = status.IsOnline,
            IsLive = status.IsLive,
            IsStatusStale = status.IsStatusStale,
            Name = status.Name,
            Type = status.Type,
            ListenUrl = status.ListenUrl,
            ListenerCount = status.ListenerCount,
            Elapsed = status.Elapsed,
            Duration = status.Duration
        };
}
