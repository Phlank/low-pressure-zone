using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Data;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Entities;
using LowPressureZone.Identity.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities.Relationships;

public class UpdateRelationship(
    DataContext dataContext,
    UserManager<AppUser> userManager)
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

        var community = await dataContext.Communities
                                         .Include(community => community.Relationships)
                                         .FirstOrDefaultAsync(community => community.Id == communityId
                                                                           && !community.IsDeleted, ct);
        var relationshipUser = await userManager.FindByIdAsync(userId.ToString());

        if (community is null || relationshipUser is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var isCommunityOrganizer = community.Relationships.Any(relationship =>
                                                                   relationship.UserId == User.GetIdOrDefault()
                                                                   && relationship.IsOrganizer);
        if (!User.IsInRole(RoleNames.Admin) && !isCommunityOrganizer)
        {
            await Send.ForbiddenAsync(ct);
            return;
        }

        var relationshipUserRoles = await userManager.GetRolesAsync(relationshipUser);
        if (relationshipUserRoles.Any(role => role == RoleNames.Admin))
        {
            await Send.ForbiddenAsync(ct);
            return;
        }

        var result = community.SetRolesForUser(userId, request.IsPerformer, request.IsOrganizer);
        await this.PublishOrThrowAsync(result);
        await dataContext.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}