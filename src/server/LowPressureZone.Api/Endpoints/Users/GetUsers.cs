using System.Diagnostics.CodeAnalysis;
using FastEndpoints;
using FluentEmail.Core;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Users;

public class GetUsers(
    IdentityContext identityContext,
    DataContext dataContext,
    IAzuraCastClient azuraCastClient,
    UserRules rules)
    : EndpointWithoutRequest<IEnumerable<UserResponse>>
{
    public override void Configure() => Get("/users");

    // This warning shows up for the LINQ expression; null-propagating operators are not allowed in expression trees, hence disabling it
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
                                IsAdmin = userRoles.Any(userRole => userRole.RoleId == adminRoleId),
                                RegistrationDate = user.Invitation != null ? user.Invitation.RegistrationDate : null,
                                CanBeDisabled = !user.LockoutEnabled,
                                CanBeEnabled = user.LockoutEnabled,
                                IsStreamer = user.StreamerId != null
                            };
        List<UserResponse> responses = await userJoinQuery.AsNoTracking()
                                                          .ToListAsync(ct);

        if (!User.IsInRole(RoleNames.Admin))
            responses = responses.Where(response => !response.IsAdmin).ToList();

        var organizerRelationships = await dataContext.CommunityRelationships
                                                      .AsNoTracking()
                                                      .Where(relationship => relationship.IsOrganizer)
                                                      .GroupBy(relationship => relationship.UserId)
                                                      .ToDictionaryAsync(group => group.Key, group => group, ct);

        foreach (var response in responses)
        {
            var relationships = organizerRelationships.GetValueOrDefault(response.Id);
            if (relationships is null) continue;
            var isOrganizer = relationships.Any(relationship => relationship.IsOrganizer);
            response.CanBeDisabled = response.CanBeDisabled &&
                                     rules.UserHasAdequateEditPermission(response.Id, response.IsAdmin, isOrganizer);
            response.CanBeEnabled = response.CanBeEnabled &&
                                    rules.UserHasAdequateEditPermission(response.Id, response.IsAdmin, isOrganizer);
        }

        await Send.OkAsync(responses, ct);
    }
}