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

    public override async Task HandleAsync(InviteRequest request, CancellationToken ct)
    {
        var invitation = Map.ToEntity(request);

        var normalizedEmail = request.Email.ToUpperInvariant().Normalize();
        var username = Guid.NewGuid().ToString();
        var normalizedUsername = username.ToUpperInvariant().Normalize();
        var user = new AppUser
        {
            Id = invitation.UserId,
            Email = request.Email,
            NormalizedEmail = normalizedEmail,
            DisplayName = username,
            UserName = username,
            NormalizedUserName = normalizedUsername
        };
        var createResult = await userManager.CreateAsync(user);
        createResult.Errors.ForEach(e => AddError(e.Code + " " + e.Description));
        ThrowIfAnyErrors();

        var inviteToken = await userManager.GenerateUserTokenAsync(user, TokenProviders.Default, TokenPurposes.Invite);
        var tokenContext = new TokenContext
        {
            Email = request.Email,
            Token = inviteToken
        };
        await emailService.SendInviteEmailAsync(request.Email, tokenContext);
        await identityContext.Invitations.AddAsync(invitation, ct);
        await identityContext.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
