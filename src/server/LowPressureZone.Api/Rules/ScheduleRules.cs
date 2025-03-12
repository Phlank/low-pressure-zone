using System.Security.Claims;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Shouldly;

namespace LowPressureZone.Api.Rules;

public class ScheduleRules(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsAddingTimeslotsAuthorized(Schedule schedule)
    {
        schedule.Audience.ShouldNotBeNull();
        schedule.Audience.Relationships.ShouldNotBeNull();
        if (schedule.EndsAt < DateTime.UtcNow) return false;
        if (User == null) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        return schedule.Audience.Relationships.Any(r => r.UserId == User.GetIdOrDefault() && r.IsPerformer);
    }

    public bool IsEditAuthorized(Schedule schedule)
    {
        schedule.Audience.ShouldNotBeNull();
        schedule.Audience.Relationships.ShouldNotBeNull();
        if (User == null) return false;
        if (schedule.EndsAt < DateTime.UtcNow) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        return schedule.Audience.Relationships.Any(r => r.UserId == User.GetIdOrDefault());
    }

    public bool IsDeleteAuthorized(Schedule schedule)
    {
        schedule.Audience.ShouldNotBeNull();
        schedule.Audience.Relationships.ShouldNotBeNull();
        if (User == null) return false;
        if (schedule.EndsAt < DateTime.UtcNow) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        return schedule.Audience.Relationships.Any(r => r.UserId == User.GetIdOrDefault());
    }

    public bool IsHiddenFromApi(Schedule schedule)
    {
        var isUserAdmin = User != null && User.IsInRole(RoleNames.Admin);
        var isScheduleInPast = schedule.EndsAt < DateTime.UtcNow;
        return isScheduleInPast && !isUserAdmin;
    }
}
