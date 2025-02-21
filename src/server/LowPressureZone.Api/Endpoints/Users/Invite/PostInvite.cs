using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Services;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Invite;

public class PostInvite : Endpoint<InviteRequest, EmptyResponse>
{
    public required UserManager<IdentityUser> UserManager { get; set; }
    public required IdentityContext IdentityContext { get; set; }
    public required EmailService EmailService { get; set; }
    public required UriService UriService { get; set; }

    public override void Configure()
    {
        Post("/users/invite");
        Roles(RoleNames.Admin);
    }

    public override async Task HandleAsync(InviteRequest req, CancellationToken ct)
    {
        var normalizedEmail = req.Email.ToUpper();
        var isInUse = IdentityContext.Users.Any(u => u.NormalizedEmail == normalizedEmail);
        if (isInUse)
        {
            ThrowError(new ValidationFailure(nameof(req.Email), Errors.Unique));
        }

        var isInvited = IdentityContext.Invitations.Any(i => !i.IsCancelled);
        if (isInvited)
        {
            ThrowError(new ValidationFailure(nameof(req.Email), Errors.EmailAlreadyInvited));
        }

        var user = new IdentityUser(Guid.NewGuid().ToString())
        {
            Email = req.Email,
            NormalizedEmail = normalizedEmail,
        };
        var createResult = await UserManager.CreateAsync(user);
        if (!createResult.Succeeded)
        {
            throw new Exception();
        }

        var inviteToken = await UserManager.GenerateUserTokenAsync(user, TokenProviders.Default, TokenPurposes.Invite);
        var inviteUrl = UriService.GetRegisterUri(req.Email, inviteToken);
        await EmailService.SendInviteEmail(req.Email, inviteUrl.AbsoluteUri);

        var invitation = new Invitation<IdentityUser>()
        {
            UserId = user.Id,
            InvitationDate = DateTime.UtcNow
        };
        await IdentityContext.Invitations.AddAsync(invitation, ct);
        await IdentityContext.SaveChangesAsync(ct);

        await SendNoContentAsync(ct);
    }
}
