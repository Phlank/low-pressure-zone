using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class GetSchedules(DataContext dataContext, ScheduleRules rules) : Endpoint<GetSchedulesRequest, IEnumerable<ScheduleResponse>, ScheduleMapper>
{
    public override void Configure()
    {
        Get("/schedules");
        Description(b => b.Produces<IEnumerable<ScheduleResponse>>(200));
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetSchedulesRequest req, CancellationToken ct)
    {
        IQueryable<Schedule> scheduleQuery = dataContext.Schedules.AsNoTracking()
                                                                  .AsSplitQuery()
                                                                  .OrderBy(s => s.StartsAt)
                                                                  .Include(s => s.Audience)
                                                                  .Include(s => s.Timeslots.OrderBy(t => t.StartsAt))
                                                                  .ThenInclude(t => t.Performer);

        if (req.Before.HasValue)
            scheduleQuery = scheduleQuery.Where(s => s.EndsAt < req.Before.Value.ToUniversalTime());
        if (req.After.HasValue)
            scheduleQuery = scheduleQuery.Where(s => s.EndsAt > req.After.Value.ToUniversalTime());

        var schedules = await scheduleQuery.ToListAsync(ct);
        schedules.RemoveAll(rules.IsHiddenFromApi);

        await SendOkAsync(schedules.Select(Map.FromEntity), ct);
    }
}
