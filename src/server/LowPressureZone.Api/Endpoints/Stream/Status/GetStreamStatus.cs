using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Api.Services.StreamStatus;

namespace LowPressureZone.Api.Endpoints.Stream.Status;

public class GetStreamStatus(IStreamStatusService streamStatusService)
    : EndpointWithoutRequest<StreamStatusResponse, StreamStatusMapper>
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
        if (status is null)
        {
            ThrowError(new ValidationFailure("", "AzuraCast server is unavailable."), 409);
        }
        ArgumentNullException.ThrowIfNull(status);
        var response = Map.FromEntity(status);
        await Send.OkAsync(response, ct);
    }
}