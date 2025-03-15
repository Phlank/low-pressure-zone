using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class GetScheduleById(DataContext dataContext, ScheduleRules rules) : EndpointWithoutRequest<ScheduleResponse, ScheduleMapper>
{
    public override void Configure()
    {
        Get("/schedules/{id}");
        Description(b => b.Produces<ScheduleResponse>(200)
                          .Produces(404));
        Roles(RoleNames.All);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var schedule = await dataContext.Schedules
                                        .AsNoTracking()
                                        .AsSplitQuery()
                                        .Include(schedule => schedule.Community)
                                        .Include(schedule => schedule.Timeslots.OrderBy(timeslot => timeslot.StartsAt))
                                        .ThenInclude(timeslot => timeslot.Performer)
                                        .Where(schedule => schedule.Id == id)
                                        .FirstOrDefaultAsync(ct);

        if (schedule == null || rules.IsHiddenFromApi(schedule))
        {
            await SendNotFoundAsync(ct);
            return;
        }
        await SendOkAsync(Map.FromEntity(schedule), ct);
    }
}
