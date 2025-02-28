﻿using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Domain.QueryableExtensions;
using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class GetScheduleById : EndpointWithoutRequest<ScheduleResponse, ScheduleResponseMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Get("/schedules/{id}");
        Description(b => b.Produces<ScheduleResponse>(200)
                          .Produces(404));
        Roles(RoleNames.All);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var schedule = DataContext.Schedules.IncludeConnectingProperties().FirstOrDefault(s => s.Id == id);
        if (schedule == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        await SendOkAsync(Map.FromEntity(schedule));
    }
}