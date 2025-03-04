using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class DeleteTimeslot : EndpointWithoutRequest
{
    private readonly DataContext _dataContext;
    private readonly TimeslotRules _rules;

    public DeleteTimeslot(DataContext dataContext, TimeslotRules rules)
    {
        _dataContext = dataContext;
        _rules = rules;
    }

    public override void Configure()
    {
        Delete("/schedules/{scheduleId}/timeslots/{timeslotId}");
        Description(b => b.Produces(204)
                          .Produces(404));
        Roles(RoleNames.All);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var timeslotId = Route<Guid>("timeslotId");
        var timeslot = await _dataContext.Timeslots.AsNoTracking()
                                                   .Include(t => t.Performer)
                                                   .Where(t => t.Id == timeslotId && t.ScheduleId == scheduleId)
                                                   .FirstOrDefaultAsync(ct);
        
        if (timeslot == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (_rules.IsDeleteAuthorized(timeslot))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await _dataContext.Timeslots.Where(t => t.Id == timeslotId).ExecuteDeleteAsync(ct);
        await SendNoContentAsync(ct);
    }
}
