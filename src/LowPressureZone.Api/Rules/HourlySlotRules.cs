using System.Security.Claims;
using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Shouldly;

namespace LowPressureZone.Api.Rules;

[RegisterService<HourlySlotRules>(LifeTime.Singleton)]
public sealed class HourlySlotRules(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsEditAuthorized(HourlySlot slot)
    {
        slot.Performer.ShouldNotBeNull();
        if (User == null) return false;
        if (slot.StartsAt < DateTime.UtcNow) return false;
        if (User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Organizer)) return true;
        return slot.Performer.CreatorUserId == User.GetIdOrDefault();
    }

    public bool IsDeleteAuthorized(HourlySlot slot)
    {
        slot.Performer.ShouldNotBeNull();
        if (User == null) return false;
        if (slot.StartsAt < DateTime.UtcNow) return false;
        if (User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Organizer)) return true;
        return slot.Performer.CreatorUserId == User.GetIdOrDefault();
    }
}