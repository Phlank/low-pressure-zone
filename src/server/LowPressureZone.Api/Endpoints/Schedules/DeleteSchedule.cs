using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class DeleteSchedule(DataContext dataContext, ScheduleRules rules) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/schedules/{id}");
        Description(builder => builder.Produces(204)
                                      .Produces(404));
        Roles(RoleNames.Admin, RoleNames.Organizer);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var schedule = await dataContext.Schedules.AsNoTracking()
                                                  .Include(s => s.Community)
                                                  .Where(s => s.Id == id)
                                                  .FirstOrDefaultAsync(ct);
        if (schedule == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (!rules.IsDeleteAuthorized(schedule))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await dataContext.Schedules.Where(s => s.Id == id).ExecuteDeleteAsync(ct);
        await SendNoContentAsync(ct);
    }
}
