using FastEndpoints;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Logout;

public class GetLogout(SignInManager<AppUser> signInManager) : EndpointWithoutRequest<EmptyResponse>
{
    public override void Configure()
    {
        Get("/users/logout");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await signInManager.SignOutAsync();
        await SendNoContentAsync(ct);
    }
}
