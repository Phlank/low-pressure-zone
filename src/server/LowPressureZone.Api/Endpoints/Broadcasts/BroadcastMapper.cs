using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Api.Rules;

namespace LowPressureZone.Api.Endpoints.Broadcasts;

public class BroadcastMapper(BroadcastRules rules) : IResponseMapper
{
    public BroadcastResponse FromEntity(StationStreamerBroadcast broadcast)
        => new()
        {
            Start = broadcast.TimestampStart,
            End = broadcast.TimestampEnd,
            BroadcastId = broadcast.Id,
            StreamerId = broadcast.Streamer?.Id,
            StreamerDisplayName = broadcast.Streamer?.DisplayName,
            IsDownloadable = rules.IsDownloadable(broadcast)
        };
}