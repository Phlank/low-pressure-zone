using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Data;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.ClashSlots;

public class DeleteClashSlot(DataContext dataContext, ClashSlotRules rules) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/schedules/{scheduleId}/clashSlots/{id}");
        Description(builder => builder.Produces(204)
                                      .Produces(404));
        Roles(RoleNames.Admin, RoleNames.Organizer);
    }

    public override async Task HandleAsync(CancellationToken ct)
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
        
        if (!rules.IsDeleteAuthorized(slot))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var result = slot.Delete();
        this.ThrowIfDomainError(result);
        await dataContext.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}