using FastEndpoints;
using LowPressureZone.Identity.Constants;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users;

public class SearchUsers : Endpoint<SearchUsersRequest, GetUserResponse, GetUserMapper>
{
    private readonly UserManager<IdentityUser> _userManager;

    public SearchUsers(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Get("/users/search");
        Roles(RoleNames.ADMIN);
        Description(builder => builder.Produces<GetUserResponse>(200)
                                      .ProducesProblem(400, "application/json+problem"));
    }

    public override async Task HandleAsync(SearchUsersRequest req, CancellationToken ct)
    {
        IdentityUser? user = null;
        if (req.Username is not null)
        {
            user = await _userManager.FindByNameAsync(req.Username);
        }
        else if (req.Email is not null)
        {
            user = await _userManager.FindByEmailAsync(req.Email);
        }

        if (user is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var roles = await _userManager.GetRolesAsync(user);
        await SendOkAsync(Map.FromEntity(user, roles));
    }
}
