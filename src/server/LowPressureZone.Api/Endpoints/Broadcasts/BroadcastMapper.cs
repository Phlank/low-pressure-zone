using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain.Entities;

namespace LowPressureZone.Api.Endpoints.Broadcasts;

public sealed class BroadcastMapper(BroadcastRules rules) : IResponseMapper
{
    public BroadcastResponse FromEntity(StationStreamerBroadcast externalBroadcast, Broadcast? broadcast = null)
        => new()
        {
            Start = externalBroadcast.TimestampStart,
            End = externalBroadcast.TimestampEnd,
            BroadcastId = externalBroadcast.Id,
            StreamerId = externalBroadcast.Streamer?.Id,
            StreamerDisplayName = externalBroadcast.Streamer?.DisplayName,
            IsDownloadable = rules.IsDownloadable(externalBroadcast),
            IsArchivable = rules.IsArchivable(externalBroadcast, broadcast),
            IsDisconnectable = rules.IsDisconnectable(externalBroadcast)
        };
}