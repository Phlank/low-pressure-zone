using FastEndpoints;
using LowPressureZone.Api.Models.Icecast;

namespace LowPressureZone.Api.Endpoints.Status.Stream;

public class StreamStatusMapper : IResponseMapper
{
    private static StreamStatusResponse OfflineResponse => new()
    {
        IsOnline = false,
        Name = null,
        Type = null,
        ListenUrl = null
    };

    public StreamStatusResponse FromEntity(IcecastStatusRaw? entity)
    {
        if (entity is null) return OfflineResponse;

        var liveSource = entity.Sources.FirstOrDefault(source => source.ListenUrl.EndsWith("/live", StringComparison.OrdinalIgnoreCase));
        if (liveSource is null) return OfflineResponse;

        return new StreamStatusResponse
        {
            IsOnline = true,
            Name = liveSource.ServerName,
            Type = liveSource.AudioInfo,
            ListenUrl = liveSource.ListenUrl
        };
    }
}
