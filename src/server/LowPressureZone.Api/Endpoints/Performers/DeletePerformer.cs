using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Performers;

public class DeletePerformer : EndpointWithoutRequest<EmptyResponse>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Delete("/performers/{id}");
        Description(builder => builder.Produces(204)
                                      .Produces(404));
        Roles(RoleNames.All);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var isLinked = DataContext.Timeslots.Any(t => t.PerformerId == id);
        if (isLinked)
        {
            ThrowError("Cannot delete performer with linked timeslots.");
        }
        var deleted = await DataContext.Performers.Where(p => p.Id == id).ExecuteDeleteAsync(ct);
        if (deleted > 0)
        {
            await SendNoContentAsync(ct);
            return;
        }
        await SendNotFoundAsync(ct);
    }
}
