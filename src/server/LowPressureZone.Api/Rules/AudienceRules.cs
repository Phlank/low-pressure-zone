using System.Security.Claims;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;

namespace LowPressureZone.Api.Rules;

public class AudienceRules(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsScheduleLinkAuthorized(Audience audience)
    {
        if (audience.IsDeleted) return false;
        if (User == null) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        return User.IsInRole(RoleNames.Organizer) && audience.LinkedUserIds.Contains(User.GetIdOrDefault());
    }

    public bool IsEditAuthorized(Audience audience)
    {
        if (audience.IsDeleted) return false;
        if (User == null) return false;
        return User.IsInRole(RoleNames.Admin);
    }

    public bool IsDeleteAuthorized(Audience audience)
    {
        if (audience.IsDeleted) return false;
        if (User == null) return false;
        return User.IsInRole(RoleNames.Admin);
    }

    public bool IsHiddenFromApi(Audience entity)
    {
        if (User == null) return true;
        return !User.IsInRole(RoleNames.Admin) && entity.IsDeleted;
    }
}
