using FastEndpoints;
using LowPressureZone.Api.Endpoints.Users.Login;
using LowPressureZone.Api.Utilities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.TwoFactor;

public class PostTwoFactor : Endpoint<TwoFactorRequest, EmptyResponse>
{
    public required SignInManager<IdentityUser> SignInManager { get; set; }

    public override void Configure()
    {
        Post("/users/twofactor");
        Throttle(5, 30);
        AllowAnonymous();
    }

    public override async Task HandleAsync(TwoFactorRequest req, CancellationToken ct)
    {
        var result = await SignInManager.TwoFactorSignInAsync("TwoFactor", req.Code, true, false);
        if (result.Succeeded)
        {
            await SendNoContentAsync(ct);
            return;
        }

        await FailOut(ct);
    }

    private DateTime _start = DateTime.UtcNow;
    private async Task FailOut(CancellationToken ct)
    {
        await TaskUtilities.DelaySensitiveResponse(_start);
        await SendUnauthorizedAsync(ct);
    }
}
