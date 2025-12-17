using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Entities;
using LowPressureZone.Identity.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities.Relationships;

public class UpdateCommunityRelationship(
    DataContext dataContext,
    IdentityContext identityContext,
    UserManager<AppUser> userManager,
    CommunityRules communityRules)
    : EndpointWithMapper<CommunityRelationshipRequest, CommunityRelationshipMapper>
{
    public override void Configure()
    {
        Verbs(Http.PUT, Http.POST);
        Roles(RoleNames.Admin, RoleNames.Organizer);
        Routes("/communities/{communityId}/relationships/{userId}");
    }

    public override async Task HandleAsync(CommunityRelationshipRequest request, CancellationToken ct)
    {
        var communityId = Route<Guid>("communityId");
        var userId = Route<Guid>("userId");

        if (!await dataContext.Communities.AnyAsync(community => community.Id == communityId, ct)
            || !await identityContext.Users.AnyAsync(user => user.Id == userId, ct))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var roles = await userManager.GetRolesAsync(user);
        if (roles.Contains(RoleNames.Admin))
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var community = await dataContext.Communities
                                         .AsNoTracking()
                                         .Where(community => community.Id == communityId)
                                         .Include(community =>
                                                      community.Relationships.Where(relationship =>
                                                                                        relationship.UserId ==
                                                                                        User.GetIdOrDefault()))
                                         .FirstAsync(ct);

        if (!communityRules.IsOrganizingAuthorized(community))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var existing = await dataContext.CommunityRelationships
                                        .Where(relationship =>
                                                   relationship.CommunityId == communityId &&
                                                   relationship.UserId == userId)
                                        .FirstOrDefaultAsync(ct);
        if (existing != null)
        {
            await Map.UpdateEntityAsync(request, existing, ct);
            await SendNoContentAsync(ct);
            return;
        }

        await dataContext.AddAsync(Map.ToEntity(request), ct);
        await dataContext.SaveChangesAsync(ct);
        HttpContext.ExposeLocation();
        await SendCreatedAtAsync<GetCommunityRelationshipById>(new
        {
            communityId,
            userId
        }, null, Http.GET, cancellation: ct);
    }
}