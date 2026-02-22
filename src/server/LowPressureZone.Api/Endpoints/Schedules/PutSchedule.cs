using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class PutSchedule(DataContext dataContext, ScheduleRules rules)
    : EndpointWithMapper<ScheduleRequest, ScheduleMapper>
{
    public override void Configure()
    {
        Put("/schedules/{id}");
        Roles(RoleNames.Admin, RoleNames.Organizer);
        Description(b => b.Produces(204)
                          .Produces(404));
    }

    public override async Task HandleAsync(ScheduleRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var schedule = await dataContext.Schedules.FirstOrDefaultAsync(schedule => schedule.Id == id, ct);
        if (schedule is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (!rules.IsEditAuthorized(schedule))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        schedule.Name = req.Name;
        schedule.StartsAt = req.StartsAt;
        schedule.EndsAt = req.EndsAt;
        schedule.CommunityId = req.CommunityId;
        schedule.Description = req.Description;
        schedule.IsOrganizersOnly = req.IsOrganizersOnly;
        schedule.LastModifiedDate = DateTime.UtcNow;
        await dataContext.SaveChangesAsync(ct);

        await Send.NoContentAsync(ct);
    }
}