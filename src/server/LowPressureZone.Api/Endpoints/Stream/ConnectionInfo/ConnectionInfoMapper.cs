using FastEndpoints;
using LowPressureZone.Api.Models.Stream;

namespace LowPressureZone.Api.Endpoints.Stream.ConnectionInfo;

public class ConnectionInfoMapper : IResponseMapper
{
    public ConnectionInfoResponse FromEntity(StreamingInfo info, string streamType)
    {
        ArgumentNullException.ThrowIfNull(info.Username);
        ArgumentNullException.ThrowIfNull(info.Password);

        return new ConnectionInfoResponse
        {
            StreamType = streamType,
            Type = info.Type,
            Host = info.Host,
            Port = info.Port,
            Username = info.Username,
            Password = info.Password,
            StreamTitleField = info.StreamTitleField
        };
    }
}
