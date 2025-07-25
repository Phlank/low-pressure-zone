using FastEndpoints;
using LowPressureZone.Api.Services.Stream;

namespace LowPressureZone.Api.Endpoints.Stream.ConnectionInfo;

public class GetConnectionInfo(ConnectionInformationService connectionInformationService)
    : EndpointWithoutRequest<IEnumerable<ConnectionInfoResponse>, ConnectionInfoMapper>
{
    public override void Configure()
        => Get("/stream/connectioninfo");

    public override async Task HandleAsync(CancellationToken ct)
    {
        var liveInfoResult = await connectionInformationService.GetLiveInfoAsync();
        if (!liveInfoResult.IsSuccess)
            ThrowError(liveInfoResult.Error ?? "The request could not be completed");

        ConnectionInfoResponse[] responses =
        [
            Map.FromEntity(liveInfoResult.Value!, "Live"),
            Map.FromEntity(connectionInformationService.GetTestInfo(), "Test")
        ];
        await SendOkAsync(responses, ct);
    }
}
