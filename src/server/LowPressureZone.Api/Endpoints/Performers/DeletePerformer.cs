using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
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
            await SendNotFoundAsync(ct);
            return;
        }

        if (!rules.IsDeleteAuthorized(performer))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        performer.IsDeleted = true;
        performer.LastModifiedDate = DateTime.UtcNow;
        await dataContext.SaveChangesAsync(ct);
        await dataContext.Timeslots.Where(timeslot => timeslot.PerformerId == performer.Id && timeslot.StartsAt > DateTime.UtcNow).ExecuteDeleteAsync(ct);
        await SendNoContentAsync(ct);
    }
}
