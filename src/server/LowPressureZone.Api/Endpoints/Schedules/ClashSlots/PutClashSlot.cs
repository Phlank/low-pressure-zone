using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Core.Domain;
using LowPressureZone.Data;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.ClashSlots;

public class PutClashSlot(DataContext dataContext, ClashSlotRules rules) : Endpoint<ClashSlotRequest>
{
    public override void Configure()
    {
        Put("/schedules/{scheduleId}/clashSlots/{id}");
        Description(b => b.Produces(204)
                          .Produces(404));
        Roles(RoleNames.Admin, RoleNames.Organizer);
    }

    public override async Task HandleAsync(ClashSlotRequest req, CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var id = Route<Guid>("id");
        
        var schedule = await dataContext.Schedules.FirstOrDefaultAsync(schedule => schedule.Id == scheduleId, ct);
        var slot = schedule?.ClashSlots.FirstOrDefault(slot => slot.Id == id);
        
        if (schedule is null || slot is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (!rules.IsEditAuthorized(slot))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }
        
        var result = DomainResult.Compose(slot.ChangeTime(req.StartsAt, req.Duration),
                                          slot.ChangeRounds(req.Rounds),
                                          slot.ChangePerformers(req.PerformerOneId, req.PerformerTwoId));
        await this.PublishOrThrowAsync(result);
        await dataContext.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }

}