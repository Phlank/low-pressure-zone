using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Microsoft.AspNetCore.Http;

namespace LowPressureZone.Domain.BusinessRules;

public class ScheduleRules
{
    private readonly IHttpContextAccessor _contextAccessor;

    public ScheduleRules(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public bool CanUserAddTimeslotsToSchedule(Schedule schedule)
    {
        if (schedule.End < DateTime.UtcNow) return false;

        var user = _contextAccessor.GetAuthenticatedUserOrDefault();
        if (user == null) return false;
        var userId = user.GetIdOrDefault();

        var dataContext = _contextAccessor.Resolve<DataContext>();
        if (!dataContext.Performers.Any(p => p.LinkedUserIds.Contains(userId))) return false;

        return true;
    }

    public bool CanUserDeleteSchedule(Schedule s)
    {
        var user = _contextAccessor.GetAuthenticatedUserOrDefault();
        if (user == null) return false;

        var dataContext = _contextAccessor.Resolve<DataContext>();
        var hasTimeslots = dataContext.Timeslots.Where(t => t.ScheduleId == s.Id).Any();
        if (hasTimeslots) return false;

        if (!user.IsInAnyRole(RoleNames.Admin, RoleNames.Organizer)) return false;
        return true;
    }

    public bool CanUserEditSchedule(Schedule s)
    {
        var user = _contextAccessor.GetAuthenticatedUserOrDefault();
        if (user == null) return false;

        if (!user.IsInAnyRole(RoleNames.Admin, RoleNames.Organizer)) return false;
        return s.End > DateTime.UtcNow;
    }
}
