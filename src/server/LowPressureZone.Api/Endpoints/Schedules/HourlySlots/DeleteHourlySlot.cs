using System.Diagnostics;
using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Data;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.HourlySlots;

public class DeleteHourlySlot(HourlySlotRules rules, DataContext dataContext) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/schedules/{scheduleId}/hourly-slots/{id}");
        Description(builder => builder.Produces(204));
        Roles(RoleNames.AllRoles);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var id = Route<Guid>("id");
        
        var schedule = await dataContext.Schedules.FirstOrDefaultAsync(schedule => schedule.Id == scheduleId, ct);
        var slot = schedule?.HourlySlots.FirstOrDefault(slot => slot.Id == id);

        if (schedule is null || slot is null)
        {
            await Send.NoContentAsync(ct);
            return;
        }

        var result = slot.Delete();
        this.ThrowIfDomainError(result);
        
        await dataContext.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}