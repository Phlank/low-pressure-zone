using System.Security.Claims;
using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain.ScheduleAggregate.ClashSlotEntity;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Shouldly;

namespace LowPressureZone.Api.Rules;

[RegisterService<ClashSlotPermissions>(LifeTime.Singleton)]
public sealed class ClashSlotPermissions(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsEditAuthorized(ClashSlot slot)
    {
        slot.Schedule.ShouldNotBeNull();
        slot.Schedule.Community.ShouldNotBeNull();
        slot.Schedule.Community.Relationships.ShouldNotBeNull();

        if (User == null) return false;
        if (slot.TimeRange.EndsAt < DateTime.UtcNow) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        return slot.Schedule.Community.Relationships.Any(r => r.IsOrganizer && r.UserId == User.GetIdOrDefault());
    }

    public bool IsDeleteAuthorized(ClashSlot slot)
    {
        slot.Schedule.ShouldNotBeNull();
        slot.Schedule.Community.ShouldNotBeNull();
        slot.Schedule.Community.Relationships.ShouldNotBeNull();

        if (User == null) return false;
        if (slot.TimeRange.StartsAt < DateTime.UtcNow) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        return slot.Schedule.Community.Relationships.Any(r => r.IsOrganizer && r.UserId == User.GetIdOrDefault());
    }
}