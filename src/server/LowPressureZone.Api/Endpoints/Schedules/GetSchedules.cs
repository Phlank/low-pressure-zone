using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Data;
using LowPressureZone.Domain.ScheduleAggregate;
using LowPressureZone.Identity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class GetSchedules(DataContext dataContext, ScheduleRules rules)
    : Endpoint<GetSchedulesRequest, IEnumerable<ScheduleResponse>, ScheduleMapper>
{
    public override void Configure()
    {
        Get("/schedules");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetSchedulesRequest req, CancellationToken ct)
    {
        IQueryable<Schedule> scheduleQuery = dataContext.NewSchedules
                                                        .GetSchedulesForResponse(User.GetIdOrDefault());

        if (req.Before.HasValue)
            scheduleQuery = scheduleQuery.Where(schedule => schedule.TimeRange.EndsAt < req.Before.Value.ToUniversalTime());
        if (req.After.HasValue)
            scheduleQuery = scheduleQuery.Where(schedule => schedule.TimeRange.EndsAt > req.After.Value.ToUniversalTime());

        var schedules = await scheduleQuery.ToListAsync(ct);
        schedules.RemoveAll(rules.IsHiddenFromApi);

        await Send.OkAsync(schedules.Select(Map.FromEntity), ct);
    }
}