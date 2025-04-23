using FastEndpoints;
using LowPressureZone.Api.Models.Icecast;

namespace LowPressureZone.Api.Endpoints.Icecast.Status;

public class IcecastStatusMapper : IResponseMapper
{
    private static IcecastStatusResponse OfflineResponse => new()
    {
        IsOnline = false,
        Name = null,
        Type = null,
        ListenUrl = null
    };

    public IcecastStatusResponse FromEntity(IcecastStatusRaw? entity)
    {
        if (entity is null) return OfflineResponse;

        var liveSource = entity.Sources.FirstOrDefault(source => source.ListenUrl.EndsWith("/live", StringComparison.OrdinalIgnoreCase));
        if (liveSource is null) return OfflineResponse;

        return new IcecastStatusResponse
        {
            IsOnline = true,
            Name = liveSource.ServerName,
            Type = liveSource.AudioInfo,
            ListenUrl = liveSource.ListenUrl
        };
    }
}
