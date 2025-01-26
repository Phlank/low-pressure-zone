using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Endpoints.Page;

public sealed class GetPage : Endpoint<GetPageRequest>
{
    private readonly DataContext _databaseContext;

    public GetPage(DataContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public override void Configure()
    {
        Get("/page/{name}");
        Roles(RoleNames.ADMIN);
    }

    public override async Task HandleAsync(GetPageRequest req, CancellationToken ct)
    {
        var page = _databaseContext.Pages.FirstOrDefault(p => p.Name == req.Name);
        if (page is null)
        {
            await SendNotFoundAsync();
            return;
        }
    }
}
