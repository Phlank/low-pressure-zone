using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Scheduling.Domain.CommunityAggregate.RelationshipEntity;

namespace LowPressureZone.Api.Endpoints.Communities.Relationships;

[RegisterService<RelationshipMapper>(LifeTime.Singleton)]
public sealed class RelationshipMapper(IHttpContextAccessor contextAccessor, RelationshipPermissions permissions)
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
            IsEditable = permissions.IsEditable(relationship, userRelationship)
        };
    }
}