using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain.BroadcastAggregate;

namespace LowPressureZone.Api.Endpoints.Broadcasts;

[RegisterService<BroadcastMapper>(LifeTime.Singleton)]
public sealed class BroadcastMapper(BroadcastPermissions permissions) : IResponseMapper
{
    public BroadcastResponse FromEntity(StationStreamerBroadcast externalBroadcast, Broadcast? broadcast = null)
        => new()
        {
            Start = externalBroadcast.TimestampStart,
            End = externalBroadcast.TimestampEnd,
            BroadcastId = externalBroadcast.Id,
            StreamerId = externalBroadcast.Streamer?.Id,
            StreamerDisplayName = externalBroadcast.Streamer?.DisplayName,
            IsDownloadable = permissions.IsDownloadable(externalBroadcast),
            IsArchivable = permissions.IsArchivable(externalBroadcast, broadcast),
            IsDisconnectable = permissions.IsDisconnectable(externalBroadcast)
        };
}