using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Domain.BusinessRules;

public class PerformerRules
{
    private readonly IHttpContextAccessor _contextAccessor;

    public PerformerRules(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    private Performer? GetPerformerOrDefault(Guid id)
    {
        var dataContext = _contextAccessor.Resolve<DataContext>();
        return dataContext.Performers.AsNoTracking()
                                     .Where(p => p.Id == id)
                                     .FirstOrDefault();
    }

    public bool IsUserLinkedToPerformer(Performer performer)
    {
        var user = _contextAccessor.GetAuthenticatedUserOrDefault();
        if (user == null) return false;
        return performer.LinkedUserIds.Contains(user.GetIdOrDefault());
    }

    public bool CanUserEditPerformer(Performer performer)
    {
        var user = _contextAccessor.GetAuthenticatedUserOrDefault();
        if (user == null) return false;
        return user.IsInRole(RoleNames.Admin) || performer.LinkedUserIds.Contains(user.GetIdOrDefault());
    }

    public bool CanUserEditPerformer(Guid performerId)
    {
        var performer = GetPerformerOrDefault(performerId);
        return performer != null && CanUserEditPerformer(performer);
    }

    public bool CanUserDeletePerformer(Performer performer)
    {
        var user = _contextAccessor.GetAuthenticatedUserOrDefault();
        if (user == null) return false;

        var dataContext = _contextAccessor.Resolve<DataContext>();
        if (dataContext.Timeslots.Any(t => t.PerformerId == performer.Id)) return false;
        
        if (user.IsInRole(RoleNames.Admin)) return true;
        return performer.LinkedUserIds.Contains(user.GetIdOrDefault());
    }

    public bool CanUserDeletePerformer(Guid performerId)
    {
        var performer = GetPerformerOrDefault(performerId);
        return performer != null && CanUserDeletePerformer(performer);
    }

    public bool IsNameInUse(string name, Guid? ignoreId = null)
    {
        ignoreId = ignoreId ?? Guid.Empty;
        var dataContext = _contextAccessor.Resolve<DataContext>();
        return dataContext.Performers.Any(p => p.Name == name && p.Id != ignoreId);
    }
}
