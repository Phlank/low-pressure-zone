using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class GetTimeslots(DataContext dataContext) : EndpointWithoutRequest<IEnumerable<TimeslotResponse>, TimeslotMapper>
{
    public override void Configure()
    {
        Get("/schedules/{scheduleId}/timeslots");
        Description(builder => builder.Produces(404));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        if (!await dataContext.HasAsync<Schedule>(scheduleId, ct))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var timeslots = await dataContext.Timeslots
                                         .AsNoTracking()
                                         .Include(timeslot => timeslot.Performer)
                                         .Where(timeslot => timeslot.ScheduleId == scheduleId)
                                         .OrderBy(timeslot => timeslot.StartsAt)
                                         .ToListAsync(ct);
        await SendOkAsync(timeslots.Select(Map.FromEntity), ct);
    }
}
