﻿using FastEndpoints;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class GetTimeslotById : EndpointWithoutRequest<TimeslotResponse, TimeslotMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Get("/schedules/{scheduleId}/timeslots/{timeslotId}");
        Description(builder => builder.Produces<TimeslotResponse>(200)
                                      .Produces(404));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var timeslotId = Route<Guid>("timeslotId");

        var timeslot = await DataContext.Timeslots.AsNoTracking()
                                                  .Include(t => t.Performer)
                                                  .Where(t => t.Id == timeslotId && t.ScheduleId == scheduleId)
                                                  .FirstOrDefaultAsync(ct);
        if (timeslot == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        var response = await Map.FromEntityAsync(timeslot, ct);
        await SendOkAsync(response, ct);
    }
}
