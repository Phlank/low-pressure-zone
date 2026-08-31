using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Core.Domain;
using LowPressureZone.Data;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class PutSchedule(DataContext dataContext, ScheduleRules rules)
    : Endpoint<ScheduleRequest>
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
        
        var schedule = await dataContext.Schedules
                                        .FirstOrDefaultAsync(schedule => schedule.Id == id, ct);
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

        var result = DomainResult.Compose(schedule.ChangeName(req.Name),
                                          schedule.ChangeDescription(req.Description),
                                          schedule.ChangeAllowedSlotTypes(req.IsHourlyAllowed, req.IsClashAllowed),
                                          schedule.ChangeVisibility(req.IsVisibleToPublic),
                                          schedule.ChangeTime(req.StartsAt, req.EndsAt));
        
        await dataContext.SaveChangesAsync(ct);

        await Send.NoContentAsync(ct);
    }
}