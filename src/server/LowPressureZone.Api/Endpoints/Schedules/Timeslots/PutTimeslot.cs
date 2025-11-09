using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class PutTimeslot(DataContext dataContext, TimeslotRules rules)
    : EndpointWithMapper<TimeslotRequest, TimeslotMapper>
{
    public override void Configure()
    {
        Put("/schedules/{scheduleId}/timeslots/{timeslotId}");
        Description(builder => builder.Produces(204)
                                      .Produces(404));
    }

    public override async Task HandleAsync(TimeslotRequest req, CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var timeslotId = Route<Guid>("timeslotId");

        var timeslot = await dataContext.Timeslots
                                        .Include(t => t.Performer)
                                        .Where(t => t.Id == timeslotId && t.ScheduleId == scheduleId)
                                        .FirstOrDefaultAsync(ct);

        if (timeslot == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (!rules.IsEditAuthorized(timeslot))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await Map.UpdateEntityAsync(req, timeslot, ct);
        await SendNoContentAsync(ct);
    }
}