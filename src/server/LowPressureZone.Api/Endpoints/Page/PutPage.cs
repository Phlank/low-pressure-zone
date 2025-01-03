using FastEndpoints;
using LowPressureZone.Identity.Constants;
using static LowPressureZone.Api.Endpoints.Page.PutPage;

namespace LowPressureZone.Api.Endpoints.Page;

public partial class PutPage : Endpoint<Request>
{
    public override void Configure()
    {
        Put("/page");
        Roles(RoleNames.ADMIN);
    }

    public override Task HandleAsync(Request req, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}
