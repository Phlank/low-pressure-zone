using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class DeleteTimeslot(DataContext dataContext, TimeslotRules rules) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/schedules/{scheduleId}/timeslots/{timeslotId}");
        Description(builder => builder.Produces(204)
                                      .Produces(404));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var timeslotId = Route<Guid>("timeslotId");
        var timeslot = await dataContext.Timeslots
                                        .AsNoTracking()
                                        .Include(t => t.Performer)
                                        .Where(t => t.Id == timeslotId && t.ScheduleId == scheduleId)
                                        .FirstOrDefaultAsync(ct);

        if (timeslot == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (rules.IsDeleteAuthorized(timeslot))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await dataContext.Timeslots.Where(t => t.Id == timeslotId).ExecuteDeleteAsync(ct);
        await SendNoContentAsync(ct);
    }
}
