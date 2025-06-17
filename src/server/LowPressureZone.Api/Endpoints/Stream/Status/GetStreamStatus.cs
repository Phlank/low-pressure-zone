using FastEndpoints;
using LowPressureZone.Api.Services.Stream;

namespace LowPressureZone.Api.Endpoints.Stream.Status;

public class GetStreamStatus(IStreamStatusService streamStatusService) : EndpointWithoutRequest<StreamStatusResponse, StreamStatusMapper>
{
    public override void Configure()
    {
        Get("/stream/status");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (!streamStatusService.IsStarted) await streamStatusService.StartAsync(ct);
        var status = streamStatusService.Status;
        if (status == null) throw new NullReferenceException();
        var response = Map.FromEntity(status!);
        await SendOkAsync(response, ct);
    }
}
