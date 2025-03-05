using System.Security.Claims;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Identity.Extensions;
using Shouldly;

namespace LowPressureZone.Api.Rules;

public class TimeslotRules(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsEditAuthorized(Timeslot timeslot)
    {
        timeslot.Performer.ShouldNotBeNull();
        if (User == null) return false;
        if (timeslot.StartsAt < DateTime.UtcNow) return false;
        return timeslot.Performer.LinkedUserIds.Contains(User.GetIdOrDefault());
    }

    public bool IsDeleteAuthorized(Timeslot timeslot)
    {
        timeslot.Performer.ShouldNotBeNull();
        if (User == null) return false;
        if (timeslot.StartsAt < DateTime.UtcNow) return false;
        return timeslot.Performer.LinkedUserIds.Contains(User.GetIdOrDefault());
    }
}
