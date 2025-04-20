using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class DeleteSchedule(DataContext dataContext, ScheduleRules rules) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/schedules/{id}");
        Roles(RoleNames.Admin, RoleNames.Organizer);
        Description(builder => builder.Produces(204)
                                      .Produces(404));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var schedule = await dataContext.Schedules
                                        .AsNoTracking()
                                        .Include(schedule => schedule.Community)
                                        .ThenInclude(community => community.Relationships.Where(relationship => relationship.UserId == User.GetIdOrDefault()))
                                        .Where(schedule => schedule.Id == id)
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
