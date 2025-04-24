using FastEndpoints;
using LowPressureZone.Api.Models.Icecast;

namespace LowPressureZone.Api.Endpoints.Icecast.Status;

public class IcecastStatusMapper : IResponseMapper
{
    private static readonly IcecastStatusResponse OfflineResponse = new()
    {
        IsOnline = false,
        IsLive = false,
        IsStale = false,
        Name = null,
        Type = null,
        ListenUrl = null
    };

    private static readonly IcecastStatusResponse NotLiveResponse = new()
    {
        IsOnline = true,
        IsLive = false,
        IsStale = false,
        Name = null,
        Type = null,
        ListenUrl = null
    };

    public IcecastStatusResponse FromEntity(IcecastStatusRaw? entity)
    {
        if (entity is null || entity.IsStale) return OfflineResponse;

        var liveSource = entity.Sources.FirstOrDefault(source => source.ListenUrl.EndsWith("/live", StringComparison.OrdinalIgnoreCase));
        if (liveSource is null) return NotLiveResponse;

        return new IcecastStatusResponse
        {
            IsOnline = true,
            IsLive = true,
            IsStale = entity.IsStale,
            Name = liveSource.ServerName,
            Type = liveSource.AudioInfo,
            ListenUrl = liveSource.ListenUrl
        };
    }
}
