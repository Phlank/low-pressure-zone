using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users;

public class GetUser : Endpoint<GetUserRequest, GetUserResponse, GetUserMapper>
{
    private readonly UserManager<IdentityUser> _userManager;
    
    public GetUser(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Get("/users/{id}");
        Description(builder => builder.Produces<GetUserResponse>(200)
                                      .Produces(404));
    }

    public override async Task HandleAsync(GetUserRequest req, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(req.Id);
        if (user is null)
        {
            await SendNotFoundAsync();
            return;
        }

        var roles = await _userManager.GetRolesAsync(user);
        await SendOkAsync(Map.FromEntity(user, roles));
    }
}
