using System.Security.Claims;
using System.Text.Json;
using FastEndpoints;

namespace LowPressureZone.Api.Endpoints.Users.Info;

public class GetUserInfo : EndpointWithoutRequest<UserResponse>
{
    public override void Configure()
    {
        Get("/users/info");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

        if (id == null || username == null || email == null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await SendOkAsync(new UserResponse()
        {
            Id = id,
            Username = username,
            Email = email,
            Roles = roles
        }, ct);
    }

    //private List<string> GetRolesFromClaim(Claim claim)
    //{
    //    var roles = new List<string>();
    //    if (claim.Value.StartsWith("["))
    //    {
    //        roles = JsonSerializer.Deserialize<List<string>>(claim.Value);
    //    }

    //}
}
