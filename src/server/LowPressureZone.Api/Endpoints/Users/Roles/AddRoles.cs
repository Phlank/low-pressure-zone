using FastEndpoints;
using LowPressureZone.Api.Extensions;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Roles;

public class AddRoles : Endpoint<AddRolesRequest>
{
    private readonly UserManager<IdentityUser> _userManager;

    public AddRoles(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post("/users/{id}/roles");
        Description(builder => builder.Produces(200)
                                      .ProducesProblem(400, "application/json+problem")
                                      .Produces(404));
    }

    public override async Task HandleAsync(AddRolesRequest req, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(req.Id);
        if (user is null)
        {
            await SendNotFoundAsync();
            return;
        }

        var result = await _userManager.AddToRolesAsync(user, req.Roles);
        this.ThrowIfIdentityErrors(result);
        await SendOkAsync();
    }
}
