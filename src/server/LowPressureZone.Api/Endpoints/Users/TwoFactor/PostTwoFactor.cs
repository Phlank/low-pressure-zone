using FastEndpoints;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.TwoFactor;

public class PostTwoFactor(SignInManager<AppUser> signInManager) : Endpoint<TwoFactorRequest, EmptyResponse>
{
    private readonly DateTime _requestStart = DateTime.UtcNow;

    public override void Configure()
    {
        Post("/users/twofactor");
        Throttle(5, 30);
        AllowAnonymous();
    }

    public override async Task HandleAsync(TwoFactorRequest req, CancellationToken ct)
    {
        var result = await signInManager.TwoFactorSignInAsync(TokenProviders.Email, req.Code, true, req.RememberClient);
        if (result.Succeeded)
        {
            await SendNoContentAsync(ct);
            return;
        }

        await this.SendDelayedUnauthorizedAsync(_requestStart, ct);
    }
}
