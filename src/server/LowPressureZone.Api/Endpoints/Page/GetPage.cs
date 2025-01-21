using FastEndpoints;
using LowPressureZone.Domain;

namespace LowPressureZone.Api.Endpoints.Page;

public sealed class GetPage : Endpoint<GetPageRequest>
{
    private readonly DatabaseContext _databaseContext;

    public GetPage(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public override void Configure()
    {
        Get("/page/{name}");
        AllowAnonymous();
    }

    public override Task HandleAsync(GetPageRequest req, CancellationToken ct)
    {
        _databaseContext.Pages.FirstOrDefault(p => p.Name == req.Name);
        return base.HandleAsync(req, ct);
    }
}
