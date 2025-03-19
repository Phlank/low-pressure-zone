using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities.Relationships;

public class GetCommunityRelationships(DataContext dataContext, IdentityContext identityContext) : EndpointWithoutRequest<IEnumerable<CommunityRelationshipResponse>, CommunityRelationshipMapper>
{
    public override void Configure()
    {
        Get("/communities/{communityId}/relationships");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var communityId = Route<Guid>("communityId");

        var relationshipJoin = await dataContext.CommunityRelationships
                                                .AsNoTracking()
                                                .Where(relationship => relationship.CommunityId == communityId)
                                                .Where(relationship => relationship.IsOrganizer || relationship.IsPerformer)
                                                .Join(identityContext.Users, relationship => relationship.UserId, user => user.Id, (relationship, user) => new
                                                {
                                                    Relationship = relationship,
                                                    user.DisplayName
                                                })
                                                .ToListAsync(ct);

        await SendOkAsync(relationshipJoin.Select(relationship => Map.FromEntity(relationship.Relationship, relationship.DisplayName)), ct);
    }
}
