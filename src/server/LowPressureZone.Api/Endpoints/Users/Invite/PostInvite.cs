using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Services;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Constants;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Invite;

public class PostInvite : Endpoint<InviteRequest, EmptyResponse>
{
    public required UserManager<IdentityUser> UserManager { get; set; }
    public required IdentityContext IdentityContext { get; set; }
    public required EmailService EmailService { get; set; }

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

        var isInvited = IdentityContext.Invitations.Any(i => !i.IsCancelled && i.NormalizedEmail == normalizedEmail);
        if (isInvited)
        {
            ThrowError(new ValidationFailure(nameof(req.Email), Errors.EmailAlreadyInvited));
        }

        await EmailService.SendInviteEmail(req.Email);
    }
}
