using FastEndpoints;
using LowPressureZone.Api.Models.Stream;

namespace LowPressureZone.Api.Endpoints.Icecast.Status;

public class StreamStatusMapper : IResponseMapper
{
    public StreamStatusResponse FromEntity(StreamStatus status)
        => new()
        {
            IsOnline = status.IsOnline,
            IsLive = status.IsLive,
            IsStatusStale = status.IsStatusStale,
            Name = status.Name,
            Type = status.Type,
            ListenUrl = status.ListenUrl
        };
}
