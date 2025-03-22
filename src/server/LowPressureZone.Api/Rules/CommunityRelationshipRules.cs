using System.Security.Claims;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Identity.Constants;
using Shouldly;

namespace LowPressureZone.Api.Rules;

public class CommunityRelationshipRules(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsEditable(CommunityRelationship communityRelationship, CommunityRelationship? userRelationship)
    {
        communityRelationship.Community.ShouldNotBeNull();
        if (User == null) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        if (userRelationship == null) return false;
        return userRelationship.IsOrganizer;
    }
}
