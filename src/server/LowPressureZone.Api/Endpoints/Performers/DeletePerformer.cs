using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Data;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Performers;

public class DeletePerformer(DataContext dataContext, PerformerRules rules) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/performers/{id}");
        Description(builder => builder.Produces(404));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var performer = await dataContext.Performers
                                         .Where(performer => performer.Id == id)
                                         .FirstOrDefaultAsync(ct);

        if (performer == null || performer.IsDeleted)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (!rules.IsDeleteAuthorized(performer))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var result = performer.Delete();
        await dataContext.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}