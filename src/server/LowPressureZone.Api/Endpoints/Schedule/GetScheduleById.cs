﻿using FastEndpoints;
using LowPressureZone.Domain;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class GetScheduleById : EndpointWithoutRequest<ScheduleResponse, ScheduleResponseMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Get("/schedules/{id}");
        Description(b => b.Produces<ScheduleResponse>(200));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var schedule = DataContext.Schedules.Find(id);
        if (schedule == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        await SendOkAsync(Map.FromEntity(schedule));
    }
}