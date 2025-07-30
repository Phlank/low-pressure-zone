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
        var liveInfoTask = connectionInformationService.GetLiveInfoAsync();
        var testInfoTask = connectionInformationService.GetTestInfoAsync();
        await Task.WhenAll(liveInfoTask, testInfoTask);
        var liveInfoResult = liveInfoTask.Result;
        if (!liveInfoResult.IsSuccess)
            ThrowError(liveInfoResult.Error);
        var testInfoResult = testInfoTask.Result;
        if (!testInfoResult.IsSuccess)
            ThrowError(testInfoResult.Error);

        ConnectionInfoResponse[] responses =
        [
            Map.FromEntity(liveInfoResult.Value, "Live Stream"),
            Map.FromEntity(testInfoResult.Value, "Test Stream")
        ];
        await SendOkAsync(responses, ct);
    }
}
