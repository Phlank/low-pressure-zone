using FastEndpoints;
using LowPressureZone.Api.Models.Stream;

namespace LowPressureZone.Api.Endpoints.Stream.ConnectionInfo;

public class ConnectionInfoMapper : IResponseMapper
{
    public ConnectionInfoResponse FromEntity(StreamingInfo info, string streamType) =>
        new()
        {
            StreamType = streamType,
            Type = info.Type,
            Host = info.Host,
            Port = info.Port,
            Mount = info.Mount,
            Username = info.Username,
            Password = info.Password,
            DisplayName = string.Empty
        };
}
