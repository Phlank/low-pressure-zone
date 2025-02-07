using FastEndpoints;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class GetPerformers : EndpointWithoutRequest<IEnumerable<PerformerResponse>, PerformerResponseMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Get("/performers");
        Description(b => b.Produces<IEnumerable<PerformerResponse>>(200));
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var performers = DataContext.Performers.AsNoTracking().ToList();
        await SendOkAsync(performers.Select(Map.FromEntity), ct);
    }
}
