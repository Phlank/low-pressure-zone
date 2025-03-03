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

public class PostInvite : Endpoint<InviteRequest, EmptyResponse>
{
    public required UserManager<AppUser> UserManager { get; set; }
    public required IdentityContext IdentityContext { get; set; }
    public required EmailService EmailService { get; set; }
    public required UriService UriService { get; set; }

    public override void Configure()
    {
        Post("/users/invite");
        Roles(RoleNames.Admin, RoleNames.Organizer);
    }

    public override async Task HandleAsync(InviteRequest req, CancellationToken ct)
    {
        ValidateAccessForAssignedRole(req);
        ThrowIfAnyErrors();

        var normalizedEmail = req.Email.ToUpper();
        var isInUse = IdentityContext.Users.Any(u => u.NormalizedEmail == normalizedEmail);
        if (isInUse)
        {
            ThrowError(new ValidationFailure(nameof(req.Email), Errors.Unique));
        }

        var user = new AppUser()
        {
            Email = req.Email,
            NormalizedEmail = normalizedEmail,
        };
        var createResult = await UserManager.CreateAsync(user);
        createResult.Errors.Select(e => e.Description).ForEach(e => AddError(e));
        ThrowIfAnyErrors();

        var addToRoleResult = await UserManager.AddToRoleAsync(user, req.Role);
        addToRoleResult.Errors.Select(e => e.Description).ForEach(e => AddError(e));
        ThrowIfAnyErrors();

        var inviteToken = await UserManager.GenerateUserTokenAsync(user, TokenProviders.Default, TokenPurposes.Invite);
        var inviteUrl = UriService.GetRegisterUri(req.Email, inviteToken);
        await EmailService.SendInviteEmail(req.Email, inviteUrl.AbsoluteUri);

        var invitation = new Invitation<Guid, AppUser>()
        {
            UserId = user.Id,
            InvitationDate = DateTime.UtcNow
        };
        await IdentityContext.Invitations.AddAsync(invitation, ct);
        await IdentityContext.SaveChangesAsync(ct);

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
