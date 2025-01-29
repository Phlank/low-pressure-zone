using FastEndpoints;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Performer;

public sealed class GetPerformer : EndpointWithoutRequest<IEnumerable<PerformerResponse>, PerformerResponseMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Get("/performer");
        Description(b => b.Produces<IEnumerable<PerformerResponse>>(200));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var performers = DataContext.Performers.AsNoTracking().ToList();
        await SendOkAsync(performers.Select(p => Map.FromEntity(p)), ct);
    }
}
