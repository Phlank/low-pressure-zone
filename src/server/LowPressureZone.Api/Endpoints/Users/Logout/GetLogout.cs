using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Logout;

public class GetLogout : EndpointWithoutRequest<EmptyResponse>
{
    public required SignInManager<IdentityUser> SignInManager { get; set; }

    public override void Configure()
    {
        Get("/users/logout");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await SignInManager.SignOutAsync();
        await SendNoContentAsync(ct);
    }
}
