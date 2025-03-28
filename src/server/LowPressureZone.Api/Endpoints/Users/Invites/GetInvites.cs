using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Users.Invites;

public class GetInvites(IdentityContext identityContext, DataContext dataContext) : EndpointWithoutRequest<IEnumerable<InviteResponse>, InviteMapper>
{
    public override void Configure()
    {
        Get("/users/invites");
        Roles(RoleNames.Admin);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var invites = await identityContext.Invitations
                                           .AsNoTracking()
                                           .Include(invitation => invitation.User)
                                           .Where(invitation => !invitation.IsCancelled && !invitation.IsRegistered)
                                           .OrderBy(invitation => invitation.InvitationDate)
                                           .ToListAsync(ct);

        var userIds = invites.Select(invitation => invitation.User!.Id);

        var userCommunities = await dataContext.CommunityRelationships
                                               .AsNoTracking()
                                               .Where(relationship => userIds.Contains(relationship.UserId))
                                               .ToDictionaryAsync(relationship => relationship.UserId, relationship => relationship.CommunityId, ct);

        var responses = invites.Where(invitation => userCommunities.ContainsKey(invitation.UserId))
                               .Select(invitation => Map.FromEntity(invitation, userCommunities[invitation.UserId]));

        await SendOkAsync(responses, ct);
    }
}
