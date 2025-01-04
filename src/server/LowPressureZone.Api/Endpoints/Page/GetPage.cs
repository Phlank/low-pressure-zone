using FastEndpoints;

namespace LowPressureZone.Api.Endpoints.Page;

public sealed class GetPage : Endpoint<GetPageRequest>
{
    public override void Configure()
    {
        Get("/page/{name}");
        AllowAnonymous();
    }

    public override Task HandleAsync(GetPageRequest req, CancellationToken ct)
    {
        return base.HandleAsync(req, ct);
    }
}
