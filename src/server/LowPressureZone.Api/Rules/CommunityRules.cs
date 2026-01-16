using System.Security.Claims;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Shouldly;

namespace LowPressureZone.Api.Rules;

public sealed class CommunityRules(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsPerformanceAuthorized(Community community)
    {
        community.Relationships.ShouldNotBeNull();
        if (community.IsDeleted) return false;
        if (User == null) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        return community.Relationships.Any(relationship =>
                                               relationship.UserId == User.GetIdOrDefault() &&
                                               relationship.IsPerformer);
    }

    public bool IsOrganizingAuthorized(Community community)
    {
        community.Relationships.ShouldNotBeNull();
        if (community.IsDeleted) return false;
        if (User == null) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        return community.Relationships.Any(relationship =>
                                               relationship.UserId == User.GetIdOrDefault() &&
                                               relationship.IsOrganizer);
    }

    public bool IsEditAuthorized(Community community)
    {
        if (community.IsDeleted) return false;
        if (User == null) return false;
        return User.IsInRole(RoleNames.Admin);
    }

    public bool IsDeleteAuthorized(Community community)
    {
        if (community.IsDeleted) return false;
        if (User == null) return false;
        return User.IsInRole(RoleNames.Admin);
    }

    public bool IsHiddenFromApi(Community community) => community.IsDeleted;
}