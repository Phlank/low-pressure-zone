using FastEndpoints;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Services;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Invites.Resend;

public class PostResendInvite(IdentityContext identityContext, UserManager<AppUser> userManager, EmailService emailService) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("/users/invites/resend/{id}");
        Roles(RoleNames.Admin);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var invite = identityContext.Invitations.FirstOrDefault(invitation => invitation.Id == id);
        if (invite == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var user = await userManager.FindByIdAsync(invite.UserId.ToString());
        if (user == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var inviteToken = await userManager.GenerateUserTokenAsync(user, TokenProviders.Default, TokenPurposes.Invite);
        var tokenContext = new TokenContext
        {
            Email = user.Email!,
            Token = inviteToken
        };
        await emailService.SendInviteEmailAsync(user.Email!, tokenContext);
        invite.LastSentDate = DateTime.UtcNow;
        await identityContext.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
