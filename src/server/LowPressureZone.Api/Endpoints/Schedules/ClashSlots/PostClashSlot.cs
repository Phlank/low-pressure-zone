using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Data;
using LowPressureZone.Domain.ScheduleAggregate.ClashSlotEntity;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.ClashSlots;

public class PostClashSlot(DataContext dataContext, ClashSlotRules rules, ScheduleRules scheduleRules)
    : Endpoint<ClashSlotRequest>
{
    public override void Configure()
    {
        Post("/schedules/{scheduleId}/clash-slots");
        Description(b => b.Produces(201));
        Roles(RoleNames.Admin, RoleNames.Organizer);
    }

    public override async Task HandleAsync(ClashSlotRequest req, CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var schedule = await dataContext.Schedules.FirstOrDefaultAsync(schedule => schedule.Id == scheduleId, ct);

        if (schedule is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (!scheduleRules.IsAddingClashSlotsAllowed(schedule))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var slotResult = ClashSlot.Create(scheduleId,
                                          req.PerformerOneId,
                                          req.PerformerTwoId,
                                          req.Rounds,
                                          req.StartsAt,
                                          req.Duration);
        await this.PublishOrThrowAsync(slotResult);
        var addToScheduleResult = schedule.AddClashSlot(slotResult.Value);
        await this.PublishOrThrowAsync(addToScheduleResult);

        await dataContext.SaveChangesAsync(ct);
        HttpContext.ExposeLocation();
        await Send.CreatedAtAsync<GetClashSlotById>(new { slotResult.Value.Id }, cancellation: ct);
    }
}