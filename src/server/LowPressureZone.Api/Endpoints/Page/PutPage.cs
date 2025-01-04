using FastEndpoints;
using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Endpoints.Page;

public sealed class PutPage : Endpoint<PutPageRequest>
{
    public override void Configure()
    {
        Put("/page");
        Roles(RoleNames.ADMIN);
    }

    public override Task HandleAsync(PutPageRequest req, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}
