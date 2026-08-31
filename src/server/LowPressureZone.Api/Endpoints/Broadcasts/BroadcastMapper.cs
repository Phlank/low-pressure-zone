using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain.BroadcastAggregate;

namespace LowPressureZone.Api.Endpoints.Broadcasts;

[RegisterService<BroadcastMapper>(LifeTime.Singleton)]
public sealed class BroadcastMapper(BroadcastPermissions permissions) : IResponseMapper
{
    public BroadcastResponse FromEntity(Broadcast broadcast)
        => new()
        {
            Start = broadcast.Time.StartsAt,
            End = broadcast.Time.EndsAt,
            BroadcastId = broadcast.AzuraCastBroadcastId,
            StreamerId = broadcast.AzuraCastStreamerId,
            StreamerDisplayName = broadcast.AzuraCastStreamerDisplayName,
            IsDownloadable = permissions.IsDownloadable(broadcast),
            IsArchivable = permissions.IsArchivable(broadcast),
            IsDisconnectable = permissions.IsDisconnectable(broadcast)
        };
}