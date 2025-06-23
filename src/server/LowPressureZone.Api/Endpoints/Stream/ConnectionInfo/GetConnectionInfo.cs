using FastEndpoints;
using LowPressureZone.Api.Services.Stream;

namespace LowPressureZone.Api.Endpoints.Stream.ConnectionInfo;

public class GetConnectionInfo(StreamInformationService streamInformationService) : EndpointWithoutRequest<IEnumerable<ConnectionInfoResponse>, ConnectionInfoMapper>
{
    public override void Configure()
        => Get("/stream/connectioninfo");

    public override async Task HandleAsync(CancellationToken ct)
    {
        ConnectionInfoResponse[] responses =
        [
            Map.FromEntity(streamInformationService.GetLiveInfo(), "Live"),
            Map.FromEntity(streamInformationService.GetTestInfo(), "Test")
        ];
        await SendOkAsync(responses, ct);
    }
}
