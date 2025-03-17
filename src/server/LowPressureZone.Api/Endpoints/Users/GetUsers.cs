using System.Diagnostics.CodeAnalysis;
using FastEndpoints;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Users;

public class GetUsers(IdentityContext identityContext) : EndpointWithoutRequest<IEnumerable<UserResponse>>
{
    public override void Configure() => Get("/users");

    [SuppressMessage("Style", "IDE0031:Use null propagation")]
    public override async Task HandleAsync(CancellationToken ct)
    {
        var adminRoleId = await identityContext.Roles
                                               .AsNoTracking()
                                               .Where(role => role.Name == RoleNames.Admin)
                                               .Select(role => role.Id)
                                               .FirstAsync(ct);

        var userJoinQuery = from user in identityContext.Users
                            join userRole in identityContext.UserRoles on user.Id equals userRole.UserId into userRoles
                            select new UserResponse
                            {
                                Id = user.Id,
                                DisplayName = user.DisplayName,
                                IsAdmin = userRoles != null && userRoles.Any(userRole => userRole.RoleId == adminRoleId),
                                RegistrationDate = user.Invitation != null ? user.Invitation.RegistrationDate : null
                            };
        if (!User.IsInRole(RoleNames.Admin)) userJoinQuery = userJoinQuery.Where(user => user.IsAdmin == false);
        var results = await userJoinQuery
                            .AsNoTracking()
                            .ToListAsync(ct);
        await SendOkAsync(results, ct);
    }
}
