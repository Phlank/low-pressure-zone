using FastEndpoints;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Users.Invites;

public class GetInvites(IdentityContext identityContext) : EndpointWithoutRequest<IEnumerable<InviteResponse>, InviteMapper>
{
    public override void Configure()
    {
        Get("/users/invites");
        Roles(RoleNames.Admin);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var invites = await identityContext.Invitations.AsNoTracking()
                                           .Include(i => i.User)
                                           .Where(i => !i.IsCancelled && !i.IsRegistered)
                                           .OrderBy(i => i.InvitationDate)
                                           .ToListAsync(ct);
        var responses = invites.Select(Map.FromEntity);
        await SendOkAsync(responses, ct);
    }
}
