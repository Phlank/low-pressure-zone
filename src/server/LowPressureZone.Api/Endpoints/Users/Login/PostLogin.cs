using FastEndpoints;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Services;
using LowPressureZone.Api.Utilities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Login;

public class PostLogin : Endpoint<LoginRequest, LoginResponse>
{
    public required SignInManager<IdentityUser> SignInManager { get; set; }
    public required UserManager<IdentityUser> UserManager { get; set; }
    public required EmailService EmailService { get; set; }
    private DateTime _start = DateTime.UtcNow;

    public override void Configure()
    {
        Post("/users/login");
        //Throttle(5, 30);
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        if (User.Identities.Any(e => e.IsAuthenticated))
        {
            await SignInManager.SignOutAsync();
        }

        if (ValidationFailed)
        {
            await FailOut(ct);
            return;
        }

        var user = await UserManager.FindByNameAsync(req.Username);
        if (user == null || user.Email == null)
        {
            await FailOut(ct);
            return;
        }

        var signInResult = await SignInManager.PasswordSignInAsync(req.Username, req.Password, true, false);

        if (signInResult.RequiresTwoFactor)
        {
            var token = await UserManager.GenerateTwoFactorTokenAsync(user, TokenProviders.Email);
            await SendOkAsync(new LoginResponse
            {
                RequiresTwoFactor = true,
            }, ct);
            await EmailService.SendTwoFactorEmail(user.Email, user.UserName!, token);
            return;
        }

        if (!signInResult.Succeeded)
        {
            await FailOut(ct);
            return;
        }

        await SendOkAsync(new LoginResponse
        {
            RequiresTwoFactor = false
        }, ct);
    }

    private async Task FailOut(CancellationToken ct)
    {
        await TaskUtilities.DelaySensitiveResponse(_start);
        await SendUnauthorizedAsync(ct);
    }
}
