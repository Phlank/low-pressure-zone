using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class PostTimeslot(DataContext dataContext, PerformerRules performerRules, ScheduleRules scheduleRules) 
    : EndpointWithMapper<TimeslotRequest, TimeslotMapper>
{
    public override void Configure()
    {
        Post("/schedules/{scheduleId}/timeslots");
        Description(b => b.Produces(201));
    }

    public override async Task HandleAsync(TimeslotRequest req, CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var schedule = await dataContext.Schedules.Include(s => s.Timeslots)
                                                  .Where(s => s.Id == scheduleId)
                                                  .FirstAsync(ct);

        var performer = await dataContext.Performers.FirstAsync(p => p.Id == req.PerformerId, ct);

        if (schedule == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (!scheduleRules.IsAddingTimeslotsAuthorized(schedule)
            || !performerRules.IsTimeslotLinkAuthorized(performer))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var timeslot = await Map.ToEntityAsync(req, ct);
        dataContext.Timeslots.Add(timeslot);
        await dataContext.SaveChangesAsync(ct);
        await SendCreatedAtAsync<GetScheduleById>(new { id = scheduleId }, Response, cancellation: ct);
    }
}
