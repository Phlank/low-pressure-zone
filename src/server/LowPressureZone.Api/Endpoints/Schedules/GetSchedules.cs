using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Identity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class GetSchedules(DataContext dataContext, ScheduleRules rules) : Endpoint<GetSchedulesRequest, IEnumerable<ScheduleResponse>, ScheduleMapper>
{
    public override void Configure()
    {
        Get("/schedules");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetSchedulesRequest req, CancellationToken ct)
    {
        IQueryable<Schedule> scheduleQuery = dataContext.Schedules
                                                        .AsNoTracking()
                                                        .AsSplitQuery()
                                                        .OrderBy(schedule => schedule.StartsAt)
                                                        .Include(schedule => schedule.Community)
                                                        .ThenInclude(community => community.Relationships.Where(relationship => relationship.UserId == User.GetIdOrDefault()))
                                                        .Include(schedule => schedule.Timeslots.OrderBy(timeslot => timeslot.StartsAt))
                                                        .ThenInclude(timeslot => timeslot.Performer);

        if (req.Before.HasValue)
            scheduleQuery = scheduleQuery.Where(s => s.EndsAt < req.Before.Value.ToUniversalTime());
        if (req.After.HasValue)
            scheduleQuery = scheduleQuery.Where(s => s.EndsAt > req.After.Value.ToUniversalTime());

        var schedules = await scheduleQuery.ToListAsync(ct);
        schedules.RemoveAll(rules.IsHiddenFromApi);

        await SendOkAsync(schedules.Select(Map.FromEntity), ct);
    }
}
