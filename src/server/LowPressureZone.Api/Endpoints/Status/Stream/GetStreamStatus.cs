using FastEndpoints;
using LowPressureZone.Api.Services;

namespace LowPressureZone.Api.Endpoints.Status.Stream;

public class GetStreamStatus(IcecastStatusService icecastStatusService) : EndpointWithoutRequest<StreamStatusResponse, StreamStatusMapper>
{
    public override void Configure()
    {
        Get("/status/stream");
        Throttle(24, 60);
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (!icecastStatusService.IsStarted) await icecastStatusService.StartAsync(ct);
        var statusRaw = icecastStatusService.Status;
        await SendOkAsync(Map.FromEntity(statusRaw), ct);
    }
}
