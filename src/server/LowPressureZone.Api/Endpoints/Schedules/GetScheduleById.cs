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
        var schedule = await dataContext.Schedules.AsNoTracking()
                                                  .AsSplitQuery()
                                                  .Include(s => s.Audience)
                                                  .Include(s => s.Timeslots.OrderBy(t => t.StartsAt))
                                                  .ThenInclude(t => t.Performer)
                                                  .Where(s => s.Id == id)
                                                  .FirstOrDefaultAsync(ct);

        if (schedule == null || rules.IsHiddenFromApi(schedule))
        {
            await SendNotFoundAsync(ct);
            return;
        }
        await SendOkAsync(await Map.FromEntityAsync(schedule, ct), ct);
    }
}