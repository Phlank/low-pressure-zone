using FastEndpoints;
using LowPressureZone.Api.Services.StreamConnectionInfo;

namespace LowPressureZone.Api.Endpoints.Stream.ConnectionInfo;

public class GetConnectionInfo(StreamingInfoService streamingInfoService)
    : EndpointWithoutRequest<IEnumerable<ConnectionInfoResponse>, ConnectionInfoMapper>
{
    public override void Configure()
        => Get("/stream/connectioninfo");

    public override async Task HandleAsync(CancellationToken ct)
    {
        var infoResult = await streamingInfoService.GetInfoAsync();
        if (!infoResult.IsSuccess)
            ThrowError(infoResult.Error);

        ConnectionInfoResponse[] responses =
        [
            Map.FromEntity(infoResult.Value.Live),
            Map.FromEntity(infoResult.Value.Test)
        ];
        await Send.OkAsync(responses, ct);
    }
}