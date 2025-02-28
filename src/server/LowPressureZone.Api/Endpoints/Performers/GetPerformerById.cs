using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class GetPerformerById : EndpointWithoutRequest<PerformerResponse, PerformerResponseMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Get("/performers/{id}");
        Description(b => b.Produces<PerformerResponse>(200)
                          .Produces(404));
        Roles(RoleNames.All);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var performer = await DataContext.Performers.AsNoTracking()
                                                    .Where(p => p.Id == id)
                                                    .FirstOrDefaultAsync(ct);
        if (performer == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        await SendOkAsync(Map.FromEntity(performer), ct);
    }
}
