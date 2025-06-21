using LowPressureZone.Api.Models.Icecast;
using LowPressureZone.Api.Models.Stream;

namespace LowPressureZone.Api.Services.Stream;

public class IcecastStatusMapper
{
    private static readonly StreamStatus Offline = new()
    {
        IsOnline = false,
        IsLive = false,
        IsStatusStale = false,
        Name = null,
        Type = null,
        ListenUrl = null
    };

    private static readonly StreamStatus NotLive = new()
    {
        IsOnline = true,
        IsLive = false,
        IsStatusStale = false,
        Name = null,
        Type = null,
        ListenUrl = null
    };

    public StreamStatus FromEntity(IcecastStatusRaw? entity)
    {
        if (entity is null || entity.IsStale) return Offline;

        var liveSource = entity.Sources.FirstOrDefault(source => source.ListenUrl.EndsWith("/live", StringComparison.OrdinalIgnoreCase));
        if (liveSource is null) return NotLive;

        return new StreamStatus
        {
            IsOnline = true,
            IsLive = true,
            IsStatusStale = entity.IsStale,
            Name = liveSource.ServerName,
            Type = liveSource.AudioInfo,
            ListenUrl = liveSource.ListenUrl
        };
    }
}
