using FastEndpoints;
using static LowPressureZone.Api.Endpoints.Page.GetPage;

namespace LowPressureZone.Api.Endpoints.Page;

public partial class GetPage : Endpoint<Request>
{
    public override void Configure()
    {
        Get("/page/{name}");
        AllowAnonymous();
    }

    public override Task HandleAsync(Request req, CancellationToken ct)
    {
        return base.HandleAsync(req, ct);
    }
}
