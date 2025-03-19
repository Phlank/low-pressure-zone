using FastEndpoints;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Services;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Login;

public class PostLogin(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, EmailService emailService) : Endpoint<LoginRequest, LoginResponse>
{
    private readonly DateTime _requestStart = DateTime.UtcNow;

    public override void Configure()
    {
        Post("/users/login");
        Throttle(10, 60);
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        if (User.Identities.Any(claimsIdentity => claimsIdentity.IsAuthenticated)) await signInManager.SignOutAsync();

        var user = await userManager.FindByNameAsync(req.Username);
        if (user?.Email == null)
        {
            await this.SendDelayedForbiddenAsync(_requestStart, ct);
            return;
        }

        var signInResult = await signInManager.PasswordSignInAsync(req.Username, req.Password, true, false);

        if (signInResult.RequiresTwoFactor)
        {
            var token = await userManager.GenerateTwoFactorTokenAsync(user, TokenProviders.Email);
            await emailService.SendTwoFactorEmailAsync(user.Email, req.Username, token);
            await SendOkAsync(new LoginResponse
            {
                RequiresTwoFactor = true
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
