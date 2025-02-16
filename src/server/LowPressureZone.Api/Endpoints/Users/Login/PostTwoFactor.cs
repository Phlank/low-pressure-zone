using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Login;

public class PostTwoFactor : Endpoint<LoginRequest, EmptyResponse>
{
    public required SignInManager<IdentityUser> SignInManager { get; set; }

    public override void Configure()
    {
        Post("/users/twofactor");
    }

    public override Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        SignInManager.Pas
    }
}
