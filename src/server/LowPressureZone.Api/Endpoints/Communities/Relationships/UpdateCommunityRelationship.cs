using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Data;
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
    : Endpoint<RelationshipRequest>
{
    public override void Configure()
    {
        Verbs(Http.PUT, Http.POST);
        Roles(RoleNames.Admin, RoleNames.Organizer);
        Routes("/communities/{communityId}/relationships/{userId}");
    }

    public override async Task HandleAsync(RelationshipRequest request, CancellationToken ct)
    {
        var communityId = Route<Guid>("communityId");
        var userId = Route<Guid>("userId");

        var community = await dataContext.NewCommunities.FirstOrDefaultAsync(community => community.Id == communityId, ct);
        var relationshipUser = await userManager.FindByIdAsync(userId.ToString());

        if (community is null || relationshipUser is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var relationshipUserRoles = await userManager.GetRolesAsync(relationshipUser);
        if (relationshipUserRoles.Any(role => role == RoleNames.Admin))
        {
            await Send.ForbiddenAsync(ct);
            return;
        }

        var result = community.SetRolesForUser(userId, request.IsPerformer, request.IsOrganizer);
        if (result.IsSuccess)
        {
            await Send.NoContentAsync(ct);
        }
    }
}