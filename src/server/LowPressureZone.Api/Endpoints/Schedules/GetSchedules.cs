using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Domain.QueryableExtensions;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class GetSchedules : EndpointWithoutRequest<IEnumerable<ScheduleResponse>, ScheduleResponseMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Get("/schedules");
        Description(b => b.Produces<IEnumerable<ScheduleResponse>>(200));
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var schedules = await DataContext.Schedules.IncludeConnectingProperties().AsNoTracking().ToListAsync(ct);
        await SendOkAsync(schedules.Select(Map.FromEntity), ct);
    }
}
