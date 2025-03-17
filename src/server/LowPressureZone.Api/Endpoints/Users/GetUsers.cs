using FastEndpoints;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Users;

public class GetUsers(IdentityContext identityContext) : EndpointWithoutRequest<IEnumerable<UserResponse>>
{
    public override void Configure() => Get("/users");

    public override async Task HandleAsync(CancellationToken ct)
    {
        var adminRoleId = await identityContext.Roles
                                               .AsNoTracking()
                                               .Where(role => role.Name == RoleNames.Admin)
                                               .Select(role => role.Id)
                                               .FirstAsync(ct);

        var userJoinQuery = from user in identityContext.Users
                            join invitation in identityContext.Invitations on user.Id equals invitation.UserId
                            join userRole in identityContext.UserRoles on user.Id equals userRole.UserId into userRoles
                            select new UserResponse
                            {
                                Id = user.Id,
                                DisplayName = user.DisplayName,
                                IsAdmin = userRoles != null && userRoles.Any(userRole => userRole.RoleId == adminRoleId),
                                RegistrationDate = invitation.RegistrationDate
                            };
        if (!User.IsInRole(RoleNames.Admin)) userJoinQuery = userJoinQuery.Where(user => user.IsAdmin == false);
        var results = await userJoinQuery
                            .AsNoTracking()
                            .ToListAsync(ct);
        await SendOkAsync(results, ct);
    }
}
