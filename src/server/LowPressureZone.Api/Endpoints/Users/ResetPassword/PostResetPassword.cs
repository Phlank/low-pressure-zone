using System.Text.Json;
using FastEndpoints;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.ResetPassword;

public class PostResetPassword(UserManager<AppUser> userManager) : Endpoint<PostResetPasswordRequest>
{
    private readonly DateTime _start = DateTime.UtcNow;

    public override void Configure()
    {
        Post("/users/resetpassword");
        AllowAnonymous();
    }

    public override async Task HandleAsync(PostResetPasswordRequest req, CancellationToken ct)
    {
        TokenContext? tokenContext;
        try
        {
            tokenContext = TokenContext.Decode(req.Context);
            if (tokenContext == null)
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

        var user = await userManager.FindByEmailAsync(tokenContext.Email);
        if (user == null)
        {
            await this.SendDelayedForbiddenAsync(_start, ct);
            return;
        }

        var isTokenVerified =
            await userManager.VerifyUserTokenAsync(user, TokenProviders.Default, TokenPurposes.ResetPassword,
                                                   tokenContext.Token);
        if (!isTokenVerified)
        {
            await this.SendDelayedForbiddenAsync(_start, ct);
            return;
        }

        if (!await userManager.HasPasswordAsync(user))
        {
            await this.SendDelayedForbiddenAsync(_start, ct);
            return;
        }

        await userManager.RemovePasswordAsync(user);
        await userManager.AddPasswordAsync(user, req.Password);
        await SendNoContentAsync(ct);
    }
}