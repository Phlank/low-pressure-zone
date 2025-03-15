using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class PutSchedule(DataContext dataContext, ScheduleRules rules) : EndpointWithMapper<ScheduleRequest, ScheduleMapper>
{
    public override void Configure()
    {
        Put("/schedules/{id}");
        Description(b => b.Produces(204)
                          .Produces(404));
        Roles(RoleNames.Admin, RoleNames.Organizer);
    }

    public override async Task HandleAsync(ScheduleRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var schedule = await dataContext.Schedules.Include(s => s.Community)
                                                  .Where(s => s.Id == id)
                                                  .FirstOrDefaultAsync(ct);
        if (schedule == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (!rules.IsEditAuthorized(schedule))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await Map.UpdateEntityAsync(req, schedule, ct);
        await SendNoContentAsync(ct);
    }
}
