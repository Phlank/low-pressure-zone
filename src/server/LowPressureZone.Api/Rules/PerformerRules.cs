using System.Security.Claims;
using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain.PerformerAggregate;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;

namespace LowPressureZone.Api.Rules;

[RegisterService<PerformerRules>(LifeTime.Singleton)]
public sealed class PerformerRules(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsHourlySlotLinkAuthorized(Performer performer)
    {
        if (performer.IsDeleted) return false;
        if (User == null) return false;
        return performer.CreatorUserId == User.GetIdOrDefault() || User.IsInRole(RoleNames.Admin);
    }

    public bool IsEditAuthorized(Performer performer)
    {
        if (performer.IsDeleted) return false;
        if (User == null) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        return performer.CreatorUserId == User.GetIdOrDefault();
    }

    public bool IsDeleteAuthorized(Performer performer)
    {
        if (performer.IsDeleted) return false;
        if (User == null) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        return performer.CreatorUserId == User.GetIdOrDefault();
    }

    public static bool IsHiddenFromApi(Performer entity)
        => entity.IsDeleted;
}