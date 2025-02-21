using FastEndpoints;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Endpoints.Users.Login;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Utilities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.TwoFactor;

public class PostTwoFactor : Endpoint<TwoFactorRequest, EmptyResponse>
{
    public required SignInManager<IdentityUser> SignInManager { get; set; }
    private DateTime _requestStart = DateTime.UtcNow;

    public override void Configure()
    {
        Post("/users/twofactor");
        Throttle(5, 30);
        AllowAnonymous();
    }

    public override async Task HandleAsync(TwoFactorRequest req, CancellationToken ct)
    {
        var result = await SignInManager.TwoFactorSignInAsync(TokenProviders.Email, req.Code, true, false);
        if (result.Succeeded)
        {
            await SendNoContentAsync(ct);
            return;
        }

        await this.SendDelayedUnauthorizedAsync(_requestStart, ct);
    }
}
