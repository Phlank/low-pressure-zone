using System.Linq;
using System.Text.Json;
using FastEndpoints;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Users;

public class GetUsers(IdentityContext identityContext) : Endpoint<GetUsersRequest, IEnumerable<UserResponse>>
{
    public override void Configure()
    {
        Get("/users");
        Description(builder => builder.Produces(200));
        Roles(RoleNames.Admin, RoleNames.Organizer);
    }

    public override async Task HandleAsync(GetUsersRequest req, CancellationToken ct)
    {
        List<string> requestRoles = req.Roles?.ToList() ?? [.. RoleNames.All];
        if (!User.IsInRole(RoleNames.Admin)) requestRoles.Remove(RoleNames.Admin);

        var roles = await identityContext.Roles.AsNoTracking()
                                               .Where(r => requestRoles.Contains(r.Name!) && r.Name != null)
                                               .Select(r => new { r.Id, r.Name })
                                               .ToDictionaryAsync(r => r.Id, r => r.Name, ct);

        var userRoles = await identityContext.UserRoles.AsNoTracking()
                                                       .Where(ur => roles.Keys.Contains(ur.RoleId))
                                                       .GroupBy(ur => ur.UserId)
                                                       .ToDictionaryAsync(grouping => grouping.Key, grouping => grouping.Select(ur => ur.RoleId), ct);

        var users = await identityContext.Users.AsNoTracking()
                                               .Where(u => userRoles.Keys.Contains(u.Id) && u.UserName != null)
                                               .Select(u => new { u.Id, u.Email, u.UserName })
                                               .ToDictionaryAsync(u => u.Id, ct);

        var responses = userRoles.Select(ur => 
        {
            return new UserResponse()
            {
                Id = ur.Key.ToString(),
                Email = users[ur.Key].Email!,
                Username = users[ur.Key].UserName!,
                Roles = ur.Value.Select(roleId => roles[roleId]!)
            };
        });
        await SendOkAsync(responses, ct);
    }
}
