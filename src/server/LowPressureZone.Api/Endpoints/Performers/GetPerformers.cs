using System.Text.Json;
using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class GetPerformers : EndpointWithoutRequest<List<PerformerResponse>, PerformerResponseMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Get("/performers");
        Description(b => b.Produces<List<PerformerResponse>>(200));
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var performers = await DataContext.Performers.AsNoTracking().ToListAsync();
        var responses = performers.Select(Map.FromEntity).ToList();
        var performerIds = performers.Select(p => p.Id).ToHashSet();
        var linkedPerformerIds = performers.Where(p => p.LinkedUserIds.Contains(User.GetIdOrDefault())).Select(p => p.Id).ToHashSet();
        var performerIdsInUse = await DataContext.Timeslots.Where(t => performerIds.Contains(t.PerformerId)).Select(t => t.PerformerId).Distinct().ToHashSetAsync();
        Console.WriteLine(JsonSerializer.Serialize(performerIdsInUse));
        foreach (var response in responses)
        {
            response.CanDelete = !performerIdsInUse.Contains(response.Id);
            response.IsLinked = linkedPerformerIds.Contains(response.Id);
        }
        await SendOkAsync(responses, ct);
    }
}
