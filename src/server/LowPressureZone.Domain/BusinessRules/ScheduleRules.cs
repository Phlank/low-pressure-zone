using System.Security.Claims;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
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

        var dataContext = _contextAccessor.ResolveDataContext();
        if (!dataContext.Performers.Any(p => p.LinkedUserIds.Contains(userId))) return false;

        return true;
    }
}
