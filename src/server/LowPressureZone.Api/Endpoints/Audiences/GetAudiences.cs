using FastEndpoints;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Audiences;

public sealed class GetAudiences : Endpoint<EmptyRequest, List<AudienceResponse>, AudienceResponseMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Get("/audiences");
        Description(b => b.Produces<List<AudienceResponse>>(200));
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var audiences = DataContext.Audiences.AsNoTracking().ToList();
        var audienceIds = audiences.Select(a => a.Id);
        var audienceIdsInSchedules = DataContext.Schedules.Where(s => audienceIds.Contains(s.AudienceId)).Select(s => s.AudienceId).Distinct().ToHashSet();

        var responses = audiences.Select(Map.FromEntity).ToList();
        foreach (var response in responses)
        {
            response.CanDelete = !audienceIdsInSchedules.Contains(response.Id);
        }
        await SendOkAsync(responses, ct);
    }
}
