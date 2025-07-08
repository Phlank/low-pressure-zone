using FastEndpoints;
using LowPressureZone.Api.Models.Stream.AzuraCast.Schema;

namespace LowPressureZone.Api.Endpoints.Broadcasts;

public class BroadcastMapper : IResponseMapper
{
    public BroadcastResponse FromEntity(Broadcast broadcast)
        => new()
        {
            Start = broadcast.TimestampStart,
            End = broadcast.TimestampEnd,
            BroadcastId = broadcast.Id,
            StreamerId = broadcast.Streamer?.Id,
            BroadcasterDisplayName = broadcast.Streamer?.DisplayName,
            IsDownloadable = broadcast.Recording is not null,
            RecordingPath = broadcast.Recording?.Path
        };
}
