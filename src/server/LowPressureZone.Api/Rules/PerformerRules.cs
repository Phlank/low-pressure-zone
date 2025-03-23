using System.Security.Claims;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;

namespace LowPressureZone.Api.Rules;

public class PerformerRules(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsTimeslotLinkAuthorized(Performer performer)
    {
        if (performer.IsDeleted) return false;
        if (User == null) return false;
        return performer.LinkedUserIds.Contains(User.GetIdOrDefault());
    }

    public bool IsEditAuthorized(Performer performer)
    {
        if (performer.IsDeleted) return false;
        if (User == null) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        return performer.LinkedUserIds.Contains(User.GetIdOrDefault());
    }

    public bool IsDeleteAuthorized(Performer performer)
    {
        if (performer.IsDeleted) return false;
        if (User == null) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        return performer.LinkedUserIds.Contains(User.GetIdOrDefault());
    }

    public static bool IsHiddenFromApi(Performer entity)
        => entity.IsDeleted;
}
