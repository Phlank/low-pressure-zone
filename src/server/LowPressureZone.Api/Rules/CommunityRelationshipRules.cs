using System.Security.Claims;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;

namespace LowPressureZone.Api.Rules;

public sealed class CommunityRelationshipRules(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsEditable(CommunityRelationship communityRelationship, CommunityRelationship? userRelationship)
    {
        if (User == null) return false;

        if (userRelationship is not null)
        {
            if (communityRelationship.CommunityId != userRelationship.CommunityId)
                throw new
                    InvalidOperationException($"{nameof(communityRelationship)} communityId does not match {nameof(userRelationship)} communityId");

            if (User.GetIdOrDefault() != userRelationship.UserId)
                throw new
                    InvalidOperationException($"{nameof(userRelationship)} userId does not match the {nameof(HttpContext)}'s userId");
        }

        if (User.IsInRole(RoleNames.Admin)) return true;
        if (userRelationship == null) return false;
        return userRelationship.IsOrganizer;
    }
}