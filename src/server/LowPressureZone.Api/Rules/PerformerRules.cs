using System.Security.Claims;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;

namespace LowPressureZone.Api.Rules;

public sealed class PerformerRules(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsTimeslotLinkAuthorized(Performer performer)
    {
        if (performer.IsDeleted) return false;
        if (User == null) return false;
        return performer.LinkedUserId == User.GetIdOrDefault();
    }

    public bool IsEditAuthorized(Performer performer)
    {
        if (performer.IsDeleted) return false;
        if (User == null) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        return performer.LinkedUserId == User.GetIdOrDefault();
    }

    public bool IsDeleteAuthorized(Performer performer)
    {
        if (performer.IsDeleted) return false;
        if (User == null) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        return performer.LinkedUserId == User.GetIdOrDefault();
    }

    public static bool IsHiddenFromApi(Performer entity)
        => entity.IsDeleted;
}