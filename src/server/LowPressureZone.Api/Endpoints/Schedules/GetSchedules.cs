using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Domain.QueryableExtensions;
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
        var schedules = await DataContext.Schedules.IncludeConnectingProperties().OrderByDescending(s => s.Start).AsNoTracking().ToListAsync(ct);
        foreach (var schedule in schedules)
        {
            schedule.Timeslots = schedule.Timeslots.OrderBy(t => t.Start).ToList();
        }
        await SendOkAsync(schedules.Select(Map.FromEntity), ct);
    }
}
