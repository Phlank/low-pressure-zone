using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;

namespace LowPressureZone.Api.Endpoints.Communities.Relationships;

public sealed class CommunityRelationshipMapper(IHttpContextAccessor contextAccessor, CommunityRelationshipRules rules)
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

    public async Task UpdateEntityAsync(
        CommunityRelationshipRequest request,
        CommunityRelationship relationship,
        CancellationToken ct = default)
    {
        var dataContext = contextAccessor.Resolve<DataContext>();
        relationship.IsPerformer = request.IsPerformer;
        relationship.IsOrganizer = request.IsOrganizer;
        if (!dataContext.ChangeTracker.HasChanges()) return;
        relationship.LastModifiedDate = DateTime.UtcNow;
        await dataContext.SaveChangesAsync(ct);
    }

    public CommunityRelationshipResponse FromEntity(
        CommunityRelationship relationship,
        string displayName,
        CommunityRelationship? userRelationship)
    {
        var communityId = contextAccessor.GetGuidRouteParameterOrDefault("communityId");
        return new CommunityRelationshipResponse
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