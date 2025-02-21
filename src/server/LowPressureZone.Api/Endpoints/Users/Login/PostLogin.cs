using FastEndpoints;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Services;
using LowPressureZone.Api.Utilities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Login;

public class PostLogin : Endpoint<LoginRequest, LoginResponse>
{
    public required SignInManager<IdentityUser> SignInManager { get; set; }
    public required UserManager<IdentityUser> UserManager { get; set; }
    public required EmailService EmailService { get; set; }
    private DateTime _requestStart = DateTime.UtcNow;

    public override void Configure()
    {
        Post("/users/login");
        Throttle(1, 5);
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        if (ValidationFailed)
        {
            await this.SendDelayedForbiddenAsync(_requestStart, ct);
            return;
        }

        if (User.Identities.Any(e => e.IsAuthenticated))
        {
            await SignInManager.SignOutAsync();
        }

        var user = await UserManager.FindByNameAsync(req.Username);
        if (user == null || user.Email == null)
        {
            await this.SendDelayedForbiddenAsync(_requestStart, ct);
            return;
        }

        var signInResult = await SignInManager.PasswordSignInAsync(req.Username, req.Password, true, false);

        if (signInResult.RequiresTwoFactor)
        {
            var token = await UserManager.GenerateTwoFactorTokenAsync(user, TokenProviders.Email);
            await EmailService.SendTwoFactorEmail(user.Email, req.Username, token);
            await SendOkAsync(new LoginResponse
            {
                RequiresTwoFactor = true,
            }, ct);
            return;
        }

        if (!signInResult.Succeeded)
        {
            await this.SendDelayedForbiddenAsync(_requestStart, ct);
            return;
        }

        await SendOkAsync(new LoginResponse
        {
            RequiresTwoFactor = false
        }, ct);
    }
}
