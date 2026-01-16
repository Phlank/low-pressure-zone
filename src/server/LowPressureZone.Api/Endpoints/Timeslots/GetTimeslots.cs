using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Timeslots;

public class GetTimeslots(DataContext dataContext)
    : Endpoint<GetTimeslotsRequest, IEnumerable<TimeslotResponse>, TimeslotMapper>
{
    public override void Configure()
    {
        Get("/timeslots");
        Description(builder => builder.Produces(404));
    }

    public override async Task HandleAsync(GetTimeslotsRequest req, CancellationToken ct)
    {
        if (!await dataContext.HasAsync<Schedule>(req.ScheduleId, ct))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var timeslots = await dataContext.Timeslots
                                         .AsNoTracking()
                                         .Include(timeslot => timeslot.Performer)
                                         .Where(timeslot => timeslot.ScheduleId == req.ScheduleId)
                                         .OrderBy(timeslot => timeslot.StartsAt)
                                         .ToListAsync(ct);
        await SendOkAsync(timeslots.Select(Map.FromEntity), ct);
    }
}