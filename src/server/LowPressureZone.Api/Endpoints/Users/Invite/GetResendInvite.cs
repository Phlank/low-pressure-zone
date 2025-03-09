using FastEndpoints;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Services;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Users.Invite;

public class GetResendInvite(UserManager<AppUser> userManager, IdentityContext identityContext, EmailService emailService) : Endpoint<GetResendInviteRequest>
{
    private readonly DateTime _start = DateTime.UtcNow;

    public override void Configure()
    {
        Get("/users/resendinvite");
        Throttle(3, 60);
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetResendInviteRequest req, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(req.Email);
        if (user == null)
        {
            await this.SendDelayedNoContentAsync(_start, ct);
            return;
        }

        var invite = await identityContext.Invitations.FirstOrDefaultAsync(i => i.UserId == user.Id && !i.IsCancelled && !i.IsRegistered, ct);
        if (invite == null)
        {
            await this.SendDelayedNoContentAsync(_start, ct);
            return;
        }

        user.Email.ShouldNotBeNullOrEmpty();
        var inviteToken = await userManager.GenerateUserTokenAsync(user, TokenProviders.Default, TokenPurposes.Invite);
        var tokenContext = new TokenContext
        {
            Email = user.Email!,
            Token = inviteToken
        };
        await emailService.SendInviteEmail(user.Email, tokenContext);
        await this.SendDelayedNoContentAsync(_start, ct);
    }
}
