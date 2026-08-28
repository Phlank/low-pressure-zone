using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Data;
using LowPressureZone.Domain.ScheduleAggregate;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class PostSchedule(DataContext dataContext, CommunityRules communityRules)
    : Endpoint<ScheduleRequest>
{
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
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var result = Schedule.Create(request.Name,
                                     request.Description,
                                     request.CommunityId,
                                     request.StartsAt,
                                     request.EndsAt,
                                     request.IsHourlyAllowed,
                                     request.IsClashAllowed,
                                     request.IsVisibleToPublic);
        
        this.ThrowIfDomainError(result);
        dataContext.Add(result.Value);
        await dataContext.SaveChangesAsync(ct);
        
        HttpContext.ExposeLocation();
        await Send.CreatedAtAsync<GetScheduleById>(new
        {
            result.Value.Id
        }, Response, cancellation: ct);
    }
}