using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class GetPerformerById(DataContext dataContext, PerformerRules rules) : EndpointWithoutRequest<PerformerResponse, PerformerMapper>
{
    public override void Configure()
    {
        Get("/performers/{id}");
        Description(b => b.Produces<PerformerResponse>(200)
                          .Produces(404));
        Roles(RoleNames.All.ToArray());
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var performer = await dataContext.Performers.AsNoTracking()
                                         .Where(p => p.Id == id)
                                         .FirstOrDefaultAsync(ct);
        if (performer == null || rules.IsHiddenFromApi(performer))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var response = Map.FromEntity(performer);
        await SendOkAsync(response, ct);
    }
}
