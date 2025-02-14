using FastEndpoints;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class GetTimeslotById : EndpointWithoutRequest<TimeslotResponse, TimeslotResponseMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Get("/schedules/{scheduleId}/timeslots/{timeslotId}");
        Description(builder => builder.Produces<TimeslotResponse>(200)
                                      .Produces(404));
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var timeslotId = Route<Guid>("timeslotId");

        var timeslot = await DataContext.Timeslots.AsNoTracking().Include("Performer").FirstOrDefaultAsync(t => t.Id == timeslotId && t.ScheduleId == scheduleId, ct);
        if (timeslot == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        await SendOkAsync(Map.FromEntity(timeslot), ct);
    }
}
