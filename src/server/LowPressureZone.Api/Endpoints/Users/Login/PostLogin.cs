using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Login;

public class PostLogin : Endpoint<LoginRequest, LoginResponse>
{
    public required SignInManager<IdentityUser> SignInManager { get; set; }

    public override void Configure()
    {
        Post("/users/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {

        var signInResult = await SignInManager.PasswordSignInAsync(req.Username, req.Password, true, false);
        if (signInResult.RequiresTwoFactor)
        {
            await SendOkAsync(new LoginResponse
            {
                RequiresTwoFactor = true,
            });
            return;
        }
    }
}
