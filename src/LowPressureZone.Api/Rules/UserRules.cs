using System.Security.Claims;
using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;

namespace LowPressureZone.Api.Rules;

[RegisterService<UserRules>(LifeTime.Singleton)]
public class UserRules(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool UserHasAdequateEditPermission(Guid userId, bool userIsAdmin, bool userIsOrganizer)
    {
        if (User is null) return false;
        if (User.GetIdOrDefault() == userId) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        if (User.IsInRole(RoleNames.Organizer)
            && !(userIsAdmin || userIsOrganizer)) return true;
        return false;
    }
}