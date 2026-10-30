using System.Security.Claims;
using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Shouldly;

namespace LowPressureZone.Api.Rules;

[RegisterService<HourlySlotPermissions>(LifeTime.Singleton)]
public sealed class HourlySlotPermissions(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsEditAuthorized(HourlySlot slot)
    {
        slot.Performer.ShouldNotBeNull();
        if (User == null) return false;
        if (slot.TimeRange.StartsAt < DateTime.UtcNow) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        return slot.Performer.CreatorUserId == User.GetIdOrDefault() || IsCommunityOrganizer(slot);
    }

    public bool IsDeleteAuthorized(HourlySlot slot)
    {
        slot.Performer.ShouldNotBeNull();
        if (User == null) return false;
        if (slot.TimeRange.StartsAt < DateTime.UtcNow) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        return slot.Performer.CreatorUserId == User.GetIdOrDefault() || IsCommunityOrganizer(slot);
    }

    private bool IsCommunityOrganizer(HourlySlot slot)
        => slot.Schedule.Community.Relationships.Any(relationship =>
                                                        relationship.UserId == User!.GetIdOrDefault()
                                                        && relationship.IsOrganizer);
}