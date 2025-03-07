using System.Text.Json;
using FastEndpoints;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Users.VerifyToken;

public class GetVerifyToken(UserManager<AppUser> userManager, IdentityContext identityContext) : Endpoint<GetVerifyTokenRequest>
{
    private readonly DateTime start = DateTime.UtcNow;

    public override void Configure()
    {
        Get("/users/verifytoken");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetVerifyTokenRequest req, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(req.Context))
        {
            await this.SendDelayedForbiddenAsync(start, ct);
            return;
        }

        TokenContext? inviteTokenContext = null;
        try
        {
            inviteTokenContext = TokenContext.Decode(req.Context);
            if (inviteTokenContext == null)
            {
                await this.SendDelayedForbiddenAsync(start, ct);
                return;
            }
        }
        catch (JsonException)
        {
            await this.SendDelayedForbiddenAsync(start, ct);
            throw;
        }

        var user = await userManager.FindByEmailAsync(inviteTokenContext.Email);
        if (user == null)
        {
            await this.SendDelayedForbiddenAsync(start, ct);
            return;
        }

        var invitation = await identityContext.Invitations.FirstOrDefaultAsync(i => i.UserId == user.Id && !i.IsRegistered && !i.IsCancelled, ct);
        if (invitation == null)
        {
            await this.SendDelayedForbiddenAsync(start, ct);
            return;
        }

        var isTokenVerified = await userManager.VerifyUserTokenAsync(user, TokenProviders.Default, req.Purpose, inviteTokenContext.Token);
        if (!isTokenVerified)
        {
            await this.SendDelayedForbiddenAsync(start, ct);
            return;
        }

        await SendNoContentAsync(ct);
    }
}
