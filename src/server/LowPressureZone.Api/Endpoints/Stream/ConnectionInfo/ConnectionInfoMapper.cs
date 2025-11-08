using FastEndpoints;
using LowPressureZone.Api.Services.StreamingInfo;

namespace LowPressureZone.Api.Endpoints.Stream.ConnectionInfo;

public sealed class ConnectionInfoMapper : IResponseMapper
{
    public ConnectionInfoResponse FromEntity(AzuraCastStreamingInfo info) => new()
    {
        StreamType = "Live Stream",
        Host = info.Host,
        Port = info.Port,
        Mount = info.Mount,
        Username = info.Username,
        Password = null,
        DisplayName = info.DisplayName,
        Type = "AzuraCast"
    };

    public ConnectionInfoResponse FromEntity(IcecastStreamingInfo info) => new()
    {
        StreamType = "Test Stream",
        Host = info.Host,
        Port = info.Port,
        Mount = info.Mount,
        Username = info.Username,
        Password = info.Password,
        DisplayName = null,
        Type = "Icecast"
    };
}