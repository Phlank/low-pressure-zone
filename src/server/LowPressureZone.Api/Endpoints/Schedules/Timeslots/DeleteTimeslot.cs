using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Domain.BusinessRules;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class DeleteTimeslot : Endpoint<EmptyRequest>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Delete("/schedules/{scheduleId}/timeslots/{timeslotId}");
        Description(b => b.Produces(204)
                          .Produces(404));
        Roles(RoleNames.All);
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var timeslotId = Route<Guid>("timeslotId");
        var timeslot = await DataContext.Timeslots.AsNoTracking()
                                                  .Where(t => t.Id == timeslotId && t.ScheduleId == scheduleId)
                                                  .Include(nameof(Timeslot.Performer))
                                                  .FirstOrDefaultAsync(ct);
        if (timeslot == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        AddBusinessRuleErrors(timeslot);
        await SendUnauthorizedAsync(ct);

    }

    private void AddBusinessRuleErrors(Timeslot timeslot)
    {
        TimeslotRules.CanUserDeleteTimeslot(User, timeslot);
    }
}
