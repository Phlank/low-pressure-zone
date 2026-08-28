using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain.CommunityAggregate.RelationshipEntity;

namespace LowPressureZone.Api.Endpoints.Communities.Relationships;

[RegisterService<CommunityRelationshipMapper>(LifeTime.Singleton)]
public sealed class CommunityRelationshipMapper(IHttpContextAccessor contextAccessor, RelationshipRules rules)
    : IResponseMapper
{
    public RelationshipResponse FromEntity(
        Relationship relationship,
        string displayName,
        Relationship? userRelationship)
    {
        var communityId = contextAccessor.GetGuidRouteParameterOrDefault("communityId");
        return new RelationshipResponse
        {
            CommunityId = communityId,
            UserId = relationship.UserId,
            DisplayName = displayName,
            IsOrganizer = relationship.IsOrganizer,
            IsPerformer = relationship.IsPerformer,
            IsEditable = rules.IsEditable(relationship, userRelationship)
        };
    }
}