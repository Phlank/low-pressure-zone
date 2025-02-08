using FastEndpoints;
using LowPressureZone.Domain;
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
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var timeslotId = Route<Guid>("timeslotId");
        var numDeletions = DataContext.Timeslots.Where(t => t.Id == timeslotId && t.ScheduleId == scheduleId).ExecuteDelete();
        if (numDeletions == 0)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        await SendNoContentAsync(ct);
    }
}
