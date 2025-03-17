using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;

namespace LowPressureZone.Api.Endpoints.Communities.Relationships;

public class CommunityRelationshipMapper(IHttpContextAccessor contextAccessor)
    : IRequestMapper, IResponseMapper
{
    public CommunityRelationship ToEntity(CommunityRelationshipRequest request)
    {
        var communityId = contextAccessor.GetGuidRouteParameterOrDefault("communityId");
        var userId = contextAccessor.GetGuidRouteParameterOrDefault("userId");
        return new CommunityRelationship
        {
            CommunityId = communityId,
            UserId = userId,
            IsOrganizer = request.IsOrganizer,
            IsPerformer = request.IsPerformer
        };
    }

    public async Task<CommunityRelationship> UpdateEntityAsync(CommunityRelationshipRequest request, CommunityRelationship relationship, CancellationToken ct = default)
    {
        var dataContext = contextAccessor.Resolve<DataContext>();
        relationship.IsPerformer = request.IsPerformer;
        relationship.IsOrganizer = request.IsOrganizer;
        if (!dataContext.ChangeTracker.HasChanges())
            return relationship;
        relationship.LastModifiedDate = DateTime.UtcNow;
        await dataContext.SaveChangesAsync(ct);
        return relationship;
    }

    public CommunityRelationshipResponse FromEntity(CommunityRelationship relationship, string displayName)
    {
        var communityId = contextAccessor.GetGuidRouteParameterOrDefault("communityId");
        var userId = contextAccessor.GetGuidRouteParameterOrDefault("userId");
        return new CommunityRelationshipResponse
        {
            CommunityId = communityId,
            UserId = userId,
            DisplayName = displayName,
            IsOrganizer = relationship.IsOrganizer,
            IsPerformer = relationship.IsPerformer
        };
    }
}
