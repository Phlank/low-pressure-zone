using FastEndpoints;
using LowPressureZone.Api.Services.Stream;

namespace LowPressureZone.Api.Endpoints.Icecast.Status;

public class GetIcecastStatus(StreamStatusService streamStatusService) : EndpointWithoutRequest<StreamStatusResponse, StreamStatusMapper>
{
    public override void Configure()
    {
        Get("/stream/status");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (!streamStatusService.IsStarted) await streamStatusService.StartAsync(ct);
        var statusRaw = streamStatusService.Status;
        await SendOkAsync(Map.FromEntity(statusRaw!), ct);
    }
}
