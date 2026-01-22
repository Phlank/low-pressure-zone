using System.Text.Json;
using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Users.VerifyToken;

public class GetVerifyToken(UserManager<AppUser> userManager, IdentityContext identityContext)
    : Endpoint<GetVerifyTokenRequest>
{
    private readonly DateTime _start = DateTime.UtcNow;

    public override void Configure()
    {
        Get("/users/verifytoken");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetVerifyTokenRequest req, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(req.Context))
        {
            await this.SendDelayedForbiddenAsync(_start, ct);
            return;
        }

        TokenContext? inviteTokenContext = null;
        try
        {
            inviteTokenContext = TokenContext.Decode(req.Context);
            if (inviteTokenContext == null)
            {
                await this.SendDelayedForbiddenAsync(_start, ct);
                return;
            }
        }
        catch (JsonException)
        {
            await this.SendDelayedForbiddenAsync(_start, ct);
            throw;
        }

        var user = await userManager.FindByEmailAsync(inviteTokenContext.Email);
        if (user == null)
        {
            await this.SendDelayedForbiddenAsync(_start, ct);
            return;
        }

        var invitation =
            await identityContext.Invitations.FirstOrDefaultAsync(i => i.UserId == user.Id && !i.IsRegistered &&
                                                                       !i.IsCancelled, ct);
        if (invitation == null)
        {
            await this.SendDelayedForbiddenAsync(_start, ct);
            return;
        }

        var isTokenVerified =
            await userManager.VerifyUserTokenAsync(user, req.Provider, req.Purpose, inviteTokenContext.Token);
        if (!isTokenVerified)
        {
            await this.SendDelayedForbiddenAsync(_start, ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }
}