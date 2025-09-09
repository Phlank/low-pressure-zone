using System.Security.Claims;
using FastEndpoints;

namespace LowPressureZone.Api.Endpoints.Users.Info;

public class GetUserInfo : EndpointWithoutRequest<UserInfoResponse>
{
    public override void Configure()
        => Get("/users/info");

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

        if (id == null || username == null || email == null)
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        await Send.OkAsync(new UserInfoResponse
        {
            Id = id,
            Username = username,
            Email = email,
            Roles = roles
        }, ct);
    }
}