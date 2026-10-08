using System.Security.Claims;
using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain.CommunityAggregate.RelationshipEntity;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;

namespace LowPressureZone.Api.Rules;

[RegisterService<RelationshipRules>(LifeTime.Singleton)]
public sealed class RelationshipRules(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsEditable(Relationship relationship, Relationship? userRelationship)
    {
        if (User == null) return false;

        if (userRelationship is not null)
        {
            if (relationship.CommunityId != userRelationship.CommunityId) 
                throw new InvalidOperationException($"CommunityIds do not match");

            if (User.GetIdOrDefault() != userRelationship.UserId)
                throw new InvalidOperationException($"UserIds do not match");
        }

        if (User.IsInRole(RoleNames.Admin)) return true;
        if (userRelationship == null) return false;
        return userRelationship.IsOrganizer;
    }
}