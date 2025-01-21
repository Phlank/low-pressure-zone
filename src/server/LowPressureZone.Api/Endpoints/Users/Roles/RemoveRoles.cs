using FastEndpoints;
using LowPressureZone.Api.Extensions;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Roles;

public class RemoveRoles : Endpoint<RemoveRolesRequest>
{
    private readonly UserManager<IdentityUser> _userManager;

    public RemoveRoles(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Delete("/users/{id}/roles");
        Description(builder => builder.Produces(204)
                                      .ProducesProblem(400, "application/json+problem")
                                      .Produces(404));
    }

    public override async Task HandleAsync(RemoveRolesRequest req, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(req.Id);
        if (user is null)
        {
            await SendNotFoundAsync();
            return;
        }

        var result = await _userManager.RemoveFromRolesAsync(user, req.Roles);
        this.ThrowIfIdentityErrors(result);
        await SendNoContentAsync();
    }
}
