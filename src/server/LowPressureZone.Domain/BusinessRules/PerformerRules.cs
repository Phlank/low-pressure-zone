using System.Security.Claims;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Microsoft.AspNetCore.Http;

namespace LowPressureZone.Domain.BusinessRules;

public class PerformerRules
{
    private readonly IHttpContextAccessor _contextAccessor;

    public PerformerRules(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public bool CanUserLinkPerformer(Performer performer)
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

    public bool CanUserDeletePerformer(Performer performer)
    {
        var user = _contextAccessor.GetAuthenticatedUserOrDefault();
        if (user == null) return false;
        return user.IsInRole(RoleNames.Admin) || performer.LinkedUserIds.Contains(user.GetIdOrDefault());
    }
}
