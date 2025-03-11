using FastEndpoints;
using FluentEmail.Core;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Endpoints.Users.Invites;
using LowPressureZone.Api.Services;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Invite;

public class PostInvite(UserManager<AppUser> userManager, IdentityContext identityContext, EmailService emailService) : EndpointWithMapper<InviteRequest, InviteMapper>
{
    public override void Configure()
    {
        Post("/users/invites");
    }

    public override async Task HandleAsync(InviteRequest req, CancellationToken ct)
    {
        ThrowIfAnyErrors();
        var invitation = await Map.ToEntityAsync(req, ct);

        var normalizedEmail = req.Email.ToUpperInvariant();
        var username = Guid.NewGuid().ToString();
        var normalizedUsername = username.ToUpperInvariant();
        var user = new AppUser()
        {
            Id = invitation.UserId,
            Email = req.Email,
            NormalizedEmail = normalizedEmail,
            UserName = username,
            NormalizedUserName = username.ToUpperInvariant()
        };
        var createResult = await userManager.CreateAsync(user);
        createResult.Errors.ForEach(e => AddError(e.Code + " " + e.Description));
        ThrowIfAnyErrors();

        var addToRoleResult = await userManager.AddToRoleAsync(user, req.Role);
        addToRoleResult.Errors.Select(e => e.Description).ForEach(e => AddError(e));
        ThrowIfAnyErrors();

        var inviteToken = await userManager.GenerateUserTokenAsync(user, TokenProviders.Default, TokenPurposes.Invite);
        var tokenContext = new TokenContext
        {
            Email = req.Email,
            Token = inviteToken,
        };
        await emailService.SendInviteEmail(req.Email, tokenContext);
        await identityContext.Invitations.AddAsync(invitation, ct);
        await identityContext.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
