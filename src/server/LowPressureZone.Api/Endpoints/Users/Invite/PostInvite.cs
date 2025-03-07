using FastEndpoints;
using FluentEmail.Core;
using FluentValidation.Results;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Services;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Invite;

public class PostInvite(UserManager<AppUser> userManager, IdentityContext identityContext, EmailService emailService) : Endpoint<InviteRequest>
{
    public override void Configure()
    {
        Post("/users/invite");
        Roles(RoleNames.Admin, RoleNames.Organizer);
    }

    public override async Task HandleAsync(InviteRequest req, CancellationToken ct)
    {
        ValidateAccessForAssignedRole(req);
        ThrowIfAnyErrors();

        var normalizedEmail = req.Email.ToUpperInvariant();
        var isInUse = identityContext.Users.Any(u => u.NormalizedEmail == normalizedEmail);
        if (isInUse)
        {
            ThrowError(new ValidationFailure(nameof(req.Email), Errors.Unique));
        }

        var username = Guid.NewGuid().ToString();
        var normalizedUsername = username.ToUpperInvariant();
        var user = new AppUser()
        {
            Email = req.Email,
            NormalizedEmail = normalizedEmail,
            UserName = username,
            NormalizedUserName = username.ToUpperInvariant()
        };
        var createResult = await userManager.CreateAsync(user);
        createResult.Errors.Select(e => e.Description).ForEach(e => AddError(e));
        ThrowIfAnyErrors();

        var addToRoleResult = await userManager.AddToRoleAsync(user, req.Role);
        addToRoleResult.Errors.Select(e => e.Description).ForEach(e => AddError(e));
        ThrowIfAnyErrors();

        var inviteToken = await userManager.GenerateUserTokenAsync(user, TokenProviders.Default, TokenPurposes.Invite);
        var tokenContext = new TokenContext
        {
            Email = req.Email,
            Token = inviteToken
        };
        await emailService.SendInviteEmail(req.Email, tokenContext);

        var invitation = new Invitation<Guid, AppUser>()
        {
            UserId = user.Id,
            InvitationDate = DateTime.UtcNow
        };
        await identityContext.Invitations.AddAsync(invitation, ct);
        await identityContext.SaveChangesAsync(ct);

        await SendNoContentAsync(ct);
    }

    private void ValidateAccessForAssignedRole(InviteRequest req)
    {
        if (User.IsInRole(RoleNames.Organizer))
        {
            if (req.Role == RoleNames.Admin) AddError(nameof(req.Role), Errors.InvalidRole);
        }
    }
}
