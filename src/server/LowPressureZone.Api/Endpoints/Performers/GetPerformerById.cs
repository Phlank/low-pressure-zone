using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class GetPerformerById : Endpoint<EmptyRequest, PerformerResponse, PerformerResponseMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Get("/performers/{id}");
        Description(b => b.Produces<PerformerResponse>(200)
                          .Produces(404));
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var performer = await DataContext.Performers.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);
        if (performer == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var response = Map.FromEntity(performer);
        response.IsDeletable = !await DataContext.Timeslots.AnyAsync(t => t.PerformerId == id, ct);
        response.IsLinkable = performer.LinkedUserIds.Contains(User.GetIdOrDefault());
        await SendOkAsync(Map.FromEntity(performer), ct);
    }
}
