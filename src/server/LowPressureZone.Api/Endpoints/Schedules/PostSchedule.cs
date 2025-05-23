﻿using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class PostSchedule(DataContext dataContext, CommunityRules communityRules) : EndpointWithMapper<ScheduleRequest, ScheduleMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Post("/schedules");
        Roles(RoleNames.Admin, RoleNames.Organizer);
        Description(b => b.Produces(201));
    }

    public override async Task HandleAsync(ScheduleRequest request, CancellationToken ct)
    {
        var community = await dataContext.Communities
                                         .Where(community => community.Id == request.CommunityId)
                                         .Include(community => community.Relationships)
                                         .FirstAsync(ct);

        if (!communityRules.IsOrganizingAuthorized(community))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var schedule = Map.ToEntity(request);

        DataContext.Schedules.Add(schedule);
        await DataContext.SaveChangesAsync(ct);
        HttpContext.ExposeLocation();
        await SendCreatedAtAsync<GetScheduleById>(new
        {
            schedule.Id
        }, Response, cancellation: ct);
    }
}
