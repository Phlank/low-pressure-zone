using System.Security.Claims;
using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain.ScheduleAggregate;
using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Rules;

[RegisterService<ScheduleRules>(LifeTime.Singleton)]
public sealed class ScheduleRules(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsAddingHourlySlotsAllowed(Schedule schedule)
    {
        if (!schedule.AllowedSlotTypes.IsHourlyAllowed) return false;
        if (schedule.TimeRange.EndsAt < DateTime.UtcNow) return false;
        if (User == null) return false;
        return User.IsInRole(RoleNames.Performer);
    }

    public bool IsAddingClashSlotsAllowed(Schedule schedule)
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