using FastEndpoints;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedule;

public class GetSchedule : EndpointWithoutRequest<IEnumerable<ScheduleResponse>, ScheduleResponseMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Get("/schedule");
        Description(b => b.Produces<IEnumerable<ScheduleResponse>>(200));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var schedules = DataContext.Schedules.AsNoTracking().ToList();
        await SendOkAsync(schedules.Select(Map.FromEntity), ct);
    }
}
