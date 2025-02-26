using System.Security.Claims;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;

namespace LowPressureZone.Domain.BusinessRules;

public static class PerformerRules
{
    public static bool CanUserLinkPerformer(ClaimsPrincipal user, Performer performer)
    {
        return IsUserLinkedOrAdmin(user, performer);
    }

    public static bool CanUserEditPerformer(ClaimsPrincipal user, Performer performer)
    {
        return IsUserLinkedOrAdmin(user, performer);
    }

    private static bool IsUserLinkedOrAdmin(ClaimsPrincipal user, Performer performer)
    {
        return user.IsInRole(RoleNames.Admin) || performer.LinkedUserIds.Contains(user.GetIdOrDefault());
    }
}
