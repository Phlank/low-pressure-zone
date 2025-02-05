using FastEndpoints;

namespace LowPressureZone.Api.Endpoints;

public class Challenge : Endpoint<EmptyRequest, EmptyResponse>
{
    public override void Configure()
    {
        Get("/challenge");
        RoutePrefixOverride("");
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        await SendOkAsync(ct);
    }
}
