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
        Roles(RoleNames.Admin);
    }

    public override async Task HandleAsync(GetUsersRequest req, CancellationToken ct)
    {
        List<string> requestRoles = req.Roles?.ToList() ?? [];
        if (requestRoles.Count == 0) requestRoles = RoleNames.All.ToList();

        var roles = await identityContext.Roles.AsNoTracking()
                                               .Where(r => requestRoles.Contains(r.Name!) && r.Name != null)
                                               .Select(r => new { r.Id, r.Name })
                                               .ToDictionaryAsync(r => r.Id, r => r.Name, ct);

        var userRoles = await identityContext.UserRoles.AsNoTracking()
                                                       .Where(ur => roles.Keys.Contains(ur.RoleId))
                                                       .GroupBy(ur => ur.UserId)
                                                       .ToDictionaryAsync(grouping => grouping.Key, grouping => grouping.Select(ur => ur.RoleId), ct);

        var users = await identityContext.Users.AsNoTracking()
                                               .Include(u => u.Invitation)
                                               .Where(u => userRoles.Keys.Contains(u.Id) && u.UserName != null)
                                               .Where(u => u.Invitation == null || u.Invitation.IsRegistered)
                                               .Select(u => new { 
                                                   u.Id,
                                                   u.Email,
                                                   u.UserName,
                                                   RegistrationDate = u.Invitation != null ? u.Invitation.RegistrationDate : null
                                               })
                                               .ToDictionaryAsync(u => u.Id, ct);

        var responses = users.Select(user =>
        {
            return new UserResponse()
            {
                Id = user.Value.Id.ToString(),
                Email = user.Value.Email!,
                Username = user.Value.UserName!,
                RegistrationDate = user.Value.RegistrationDate,
                Roles = userRoles[user.Value.Id].Select(roleId => roles[roleId]!)
            };
        });
        await SendOkAsync(responses, ct);
    }
}
