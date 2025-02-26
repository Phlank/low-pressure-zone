using LowPressureZone.Domain.Entities;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using System.Security.Claims;

namespace LowPressureZone.Domain.BusinessRules;

public static class TimeslotRules
{
    public static bool CanUserDeleteTimeslot(ClaimsPrincipal user, Timeslot timeslot)
    {
        if (timeslot.Performer == null) return user.IsInRole(RoleNames.Admin);
        return PerformerRules.CanUserLinkPerformer(user, timeslot.Performer);
    }

    public static bool CanUserEditTimeslot(ClaimsPrincipal user, Timeslot timeslot)
    {
        if (timeslot.Performer == null) return user.IsInRole(RoleNames.Admin);
        return PerformerRules.CanUserLinkPerformer(user, timeslot.Performer);
    }
}
