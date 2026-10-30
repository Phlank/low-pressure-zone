using System.Security.Claims;
using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;

namespace LowPressureZone.Api.Rules;

[RegisterService<SchedulePermissions>(LifeTime.Singleton)]
public sealed class SchedulePermissions(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsAddingHourlySlotsAllowed(Schedule schedule)
    {
        if (!schedule.AllowedSlotTypes.IsHourlyAllowed) return false;
        if (schedule.TimeRange.EndsAt < DateTime.UtcNow) return false;
        if (User == null) return false;
         return User.IsInRole(RoleNames.Performer) || User.IsInRole(RoleNames.Admin);
    }

    public bool IsAddingClashSlotsAllowed(Schedule schedule)
    {
        if (!schedule.AllowedSlotTypes.IsClashAllowed) return false;
        if (schedule.TimeRange.EndsAt < DateTime.UtcNow) return false;
        if (User == null) return false;
        return User.IsInRole(RoleNames.Admin) || IsCommunityOrganizer(schedule);
    }

    public bool IsEditAuthorized(Schedule schedule)
    {
        if (User == null) return false;
        if (schedule.TimeRange.EndsAt < DateTime.UtcNow) return false;
        return User.IsInRole(RoleNames.Admin) || IsCommunityOrganizer(schedule);
    }

    public bool IsDeleteAuthorized(Schedule schedule)
    {
        if (User == null) return false;
        if (schedule.TimeRange.EndsAt < DateTime.UtcNow) return false;
        return User.IsInRole(RoleNames.Admin) || IsCommunityOrganizer(schedule);
    }

    public bool IsHiddenFromApi(Schedule schedule) => User is null && !schedule.IsVisibleToPublic;

    private bool IsCommunityOrganizer(Schedule schedule)
        => schedule.Community.Relationships.Any(relationship =>
                                                    relationship.UserId == User!.GetIdOrDefault()
                                                    && relationship.IsOrganizer);
}