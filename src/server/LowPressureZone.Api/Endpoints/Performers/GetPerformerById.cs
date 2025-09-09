using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class GetPerformerById(DataContext dataContext)
    : EndpointWithoutRequest<PerformerResponse, PerformerMapper>
{
    public override void Configure()
    {
        Get("/performers/{id}");
        Description(builder => builder.Produces(404));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var performer = await dataContext.Performers
                                         .AsNoTracking()
                                         .Where(p => p.Id == id)
                                         .FirstOrDefaultAsync(ct);
        if (performer == null || PerformerRules.IsHiddenFromApi(performer))
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var response = Map.FromEntity(performer);
        await Send.OkAsync(response, ct);
    }
}