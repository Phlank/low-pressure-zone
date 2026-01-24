using System.Security.Claims;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Shouldly;

namespace LowPressureZone.Api.Rules;

public sealed class TimeslotRules(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsEditAuthorized(Timeslot timeslot)
    {
        timeslot.Performer.ShouldNotBeNull();
        if (User == null) return false;
        if (timeslot.StartsAt < DateTime.UtcNow) return false;
        if (User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Organizer)) return true;
        return timeslot.Performer.LinkedUserIds.Contains(User.GetIdOrDefault());
    }

    public bool IsDeleteAuthorized(Timeslot timeslot)
    {
        timeslot.Performer.ShouldNotBeNull();
        if (User == null) return false;
        if (timeslot.StartsAt < DateTime.UtcNow) return false;
        if (User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Organizer)) return true;
        return timeslot.Performer.LinkedUserIds.Contains(User.GetIdOrDefault());
    }
}