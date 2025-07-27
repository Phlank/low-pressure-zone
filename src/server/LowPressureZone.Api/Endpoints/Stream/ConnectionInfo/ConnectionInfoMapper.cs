using FastEndpoints;
using LowPressureZone.Api.Models.Stream.Info;

namespace LowPressureZone.Api.Endpoints.Stream.ConnectionInfo;

public sealed class ConnectionInfoMapper : IResponseMapper
{
    public ConnectionInfoResponse FromEntity(StreamingInfo info, string streamType) =>
        new()
        {
            StreamType = streamType,
            Type = info.Type.ToString(),
            Host = info.Host,
            Port = info.Port,
            Mount = info.Mount,
            Username = info.Username,
            Password = info.Password,
            DisplayName = info.DisplayName,
            IsDisplayNameEditable = info.IsDisplayNameEditable
        };
}
