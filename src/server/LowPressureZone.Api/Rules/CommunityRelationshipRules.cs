using System.Security.Claims;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Shouldly;

namespace LowPressureZone.Api.Rules;

public class CommunityRelationshipRules(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsEditable(CommunityRelationship communityRelationship)
    {
        communityRelationship.Community.ShouldNotBeNull();
        if (User == null) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        var isUserOrganizer = communityRelationship.Community.Relationships.Any(relationship => relationship.UserId == User.GetIdOrDefault() && relationship.IsOrganizer);
        return !communityRelationship.IsOrganizer && isUserOrganizer;
    }
}
