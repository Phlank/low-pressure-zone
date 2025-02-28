using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class GetTimeslots : EndpointWithoutRequest<IEnumerable<TimeslotResponse>, TimeslotResponseMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Get("/schedules/{scheduleId}/timeslots");
        Description(builder => builder.Produces<IEnumerable<TimeslotResponse>>(200)
                                      .Produces(404));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        if (!DataContext.Has<Schedule>(scheduleId))
        {
            await SendNotFoundAsync(ct);
            return;
        }
        var timeslots = await DataContext.Timeslots.AsNoTracking().Include("Performer").Where(t => t.ScheduleId == scheduleId).OrderBy(t => t.Start).ToListAsync();
        await SendOkAsync(timeslots.Select(Map.FromEntity));
    }
}
