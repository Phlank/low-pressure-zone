using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Domain.QueryableExtensions;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class GetSchedules : Endpoint<GetSchedulesRequest, IEnumerable<ScheduleResponse>, ScheduleResponseMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Get("/schedules");
        Description(b => b.Produces<IEnumerable<ScheduleResponse>>(200));
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetSchedulesRequest req, CancellationToken ct)
    {
        var scheduleQuery = DataContext.Schedules.IncludeConnectingProperties().AsNoTracking();
        
        if (req.Before.HasValue)
        {
            scheduleQuery = scheduleQuery.Where(s => s.End < req.Before.Value.ToUniversalTime());
        }
        if (req.After.HasValue)
        {
            scheduleQuery = scheduleQuery.Where(s => s.End > req.After.Value.ToUniversalTime());
        }

        if (!User.IsInAnyRole(RoleNames.Admin, RoleNames.Organizer))
        {
            scheduleQuery = scheduleQuery.Where(s  => s.End > DateTime.UtcNow);
        }

        var schedules = await scheduleQuery.ToListAsync(ct);
        foreach (var schedule in schedules)
        {
            schedule.Timeslots = schedule.Timeslots.OrderBy(t => t.Start).ToList();
        }
        await SendOkAsync(schedules.Select(Map.FromEntity), ct);
    }
}
