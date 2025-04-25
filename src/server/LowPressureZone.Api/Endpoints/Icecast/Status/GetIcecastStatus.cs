using FastEndpoints;
using LowPressureZone.Api.Services;

namespace LowPressureZone.Api.Endpoints.Icecast.Status;

public class GetIcecastStatus(IcecastStatusService icecastStatusService) : EndpointWithoutRequest<IcecastStatusResponse, IcecastStatusMapper>
{
    public override void Configure()
    {
        Get("/icecast/status");
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
