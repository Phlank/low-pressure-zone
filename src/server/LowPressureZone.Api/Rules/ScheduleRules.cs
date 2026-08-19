using System.Security.Claims;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain.ScheduleAggregate;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;

namespace LowPressureZone.Api.Rules;

public sealed class ScheduleRules(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsAddingTimeslotsAllowed(Schedule schedule)
    {
        if (!schedule.AllowedSlotTypes.IsHourlyAllowed) return false;
        if (schedule.TimeRange.EndsAt < DateTime.UtcNow) return false;
        if (User == null) return false;
        return User.IsInRole(RoleNames.Performer);
    }

    public bool IsAddingSoundclashesAllowed(Schedule schedule)
    {
        if (!schedule.AllowedSlotTypes.IsClashAllowed) return false;
        if (schedule.TimeRange.EndsAt < DateTime.UtcNow) return false;
        if (User == null) return false;
        return User.IsInRole(RoleNames.Organizer) || User.IsInRole(RoleNames.Admin);
    }

    public bool IsEditAuthorized(Schedule schedule)
    {
        if (User == null) return false;
        if (schedule.TimeRange.EndsAt < DateTime.UtcNow) return false;
        return User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Organizer);
    }

    public bool IsDeleteAuthorized(Schedule schedule)
    {
        if (User == null) return false;
        if (schedule.TimeRange.EndsAt < DateTime.UtcNow) return false;
        return User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Organizer);
    }

    public bool IsHiddenFromApi(Schedule schedule)
    {
        var isUserAdminOrOrganizer =
            User != null && (User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Organizer));
        var isScheduleInPast = schedule.TimeRange.EndsAt < DateTime.UtcNow;
        return !isUserAdminOrOrganizer
               && (isScheduleInPast 
                   || schedule.IsVisibleToPublic);
    }
}