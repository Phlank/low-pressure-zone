using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Domain.BusinessRules;

public class TimeslotRules
{
    private readonly IHttpContextAccessor _contextAccessor;

    public TimeslotRules(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public bool CanUserDeleteTimeslot(Timeslot timeslot)
    {
        var user = _contextAccessor.GetAuthenticatedUserOrDefault();
        if (user == null) return false;

        var dataContext = _contextAccessor.Resolve<DataContext>();
        var performer = timeslot.Performer ?? dataContext.Performers.AsNoTracking()
                                                                    .Where(p => p.Id == timeslot.PerformerId)
                                                                    .FirstOrDefault();
        if (performer == null) return false;

        return user.IsInRole(RoleNames.Admin) || performer.LinkedUserIds.Contains(user.GetIdOrDefault());
    }

    public bool CanUserEditTimeslot(Timeslot timeslot)
    {
        var user = _contextAccessor.GetAuthenticatedUserOrDefault();
        if (user == null) return false;

        var dataContext = _contextAccessor.Resolve<DataContext>();
        var performer = timeslot.Performer ?? dataContext.Performers.AsNoTracking()
                                                                    .Where(p => p.Id == timeslot.PerformerId)
                                                                    .FirstOrDefault();
        if (performer == null) return false;

        return performer.LinkedUserIds.Contains(user.GetIdOrDefault());
    }

    public bool CanUserLinkPerformerToTimeslot(Performer performer)
    {
        var user = _contextAccessor.GetAuthenticatedUserOrDefault();
        if (user == null) return false;

        return performer.LinkedUserIds.Contains(user.GetIdOrDefault());
    }
}
