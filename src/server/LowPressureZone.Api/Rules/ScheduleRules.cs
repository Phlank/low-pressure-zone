using System.Security.Claims;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Shouldly;

namespace LowPressureZone.Api.Rules;

public class ScheduleRules(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsAddingTimeslotsAuthorized(Schedule schedule)
    {
        schedule.Community.ShouldNotBeNull();
        schedule.Community.Relationships.ShouldNotBeNull();
        if (schedule.EndsAt < DateTime.UtcNow) return false;
        if (User == null) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        return schedule.Community.Relationships.Any(relationship =>
                                                        relationship.UserId == User.GetIdOrDefault() &&
                                                        relationship.IsPerformer);
    }

    public bool IsEditAuthorized(Schedule schedule)
    {
        schedule.Community.ShouldNotBeNull();
        schedule.Community.Relationships.ShouldNotBeNull();
        if (User == null) return false;
        if (schedule.EndsAt < DateTime.UtcNow) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        return schedule.Community.Relationships.Any(relationship =>
                                                        relationship.UserId == User.GetIdOrDefault() &&
                                                        relationship.IsOrganizer);
    }

    public bool IsDeleteAuthorized(Schedule schedule)
    {
        schedule.Community.ShouldNotBeNull();
        schedule.Community.Relationships.ShouldNotBeNull();
        if (User == null) return false;
        if (schedule.EndsAt < DateTime.UtcNow) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        return schedule.Community.Relationships.Any(relationship =>
                                                        relationship.UserId == User.GetIdOrDefault() &&
                                                        relationship.IsOrganizer);
    }

    public bool IsHiddenFromApi(Schedule schedule)
    {
        var isUserAdminOrOrganizer =
            User != null && (User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Organizer));
        var isScheduleInPast = schedule.EndsAt < DateTime.UtcNow;
        return isScheduleInPast && !isUserAdminOrOrganizer;
    }
}
