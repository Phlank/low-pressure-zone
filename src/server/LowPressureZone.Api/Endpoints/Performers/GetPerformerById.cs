using FastEndpoints;
using LowPressureZone.Domain;
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
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var performer = DataContext.Performers.AsNoTracking().FirstOrDefault(p => p.Id == id);
        if (performer == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        await SendOkAsync(Map.FromEntity(performer), ct);
    }
}
