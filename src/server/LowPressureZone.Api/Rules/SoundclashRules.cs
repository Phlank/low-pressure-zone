using System.Security.Claims;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Shouldly;

namespace LowPressureZone.Api.Rules;

public sealed class SoundclashRules(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsEditAuthorized(Soundclash soundclash)
    {
        soundclash.Schedule.ShouldNotBeNull();
        soundclash.Schedule.Community.ShouldNotBeNull();
        soundclash.Schedule.Community.Relationships.ShouldNotBeNull();
        
        if (User == null) return false;
        if (soundclash.EndsAt < DateTime.UtcNow) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        if (soundclash.Schedule.Community.Relationships.Any(r => r.IsOrganizer && r.UserId == User.GetIdOrDefault()))
            return true;
        return false;
    }

    public bool IsDeleteAuthorized(Soundclash soundclash)
    {
        soundclash.Schedule.ShouldNotBeNull();
        soundclash.Schedule.Community.ShouldNotBeNull();
        soundclash.Schedule.Community.Relationships.ShouldNotBeNull();
        
        if (User == null) return false;
        if (soundclash.StartsAt < DateTime.UtcNow) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        if (soundclash.Schedule.Community.Relationships.Any(r => r.IsOrganizer && r.UserId == User.GetIdOrDefault()))
            return true;
        return false;
    }
}