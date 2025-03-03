using System.Text.Json;
using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class GetPerformers : EndpointWithoutRequest<IEnumerable<PerformerResponse>, PerformerResponseMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Get("/performers");
        Description(b => b.Produces<List<PerformerResponse>>(200));
        Roles(RoleNames.All.ToArray());
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var performers = await DataContext.Performers.AsNoTracking()
                                                     .OrderBy(p => p.Name)
                                                     .ToListAsync();
        var responses = performers.Select(Map.FromEntity).ToList();
        await SendOkAsync(responses, ct);
    }
}
