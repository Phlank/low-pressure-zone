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

        var relationships = await dataContext.CommunityRelationships
                                             .AsNoTracking()
                                             .Where(relationship => relationship.CommunityId == communityId)
                                             .ToListAsync(ct);
        var userIds = relationships.Select(relationship => relationship.UserId);
        var usernameDictionary = await identityContext.Users
                                                      .AsNoTracking()
                                                      .Where(user => userIds.Contains(user.Id))
                                                      .ToDictionaryAsync(user => user.Id, user => user.UserName!, ct);

        await SendOkAsync(relationships.Select(relationship => Map.FromEntity(relationship, usernameDictionary[relationship.UserId])), ct);
    }
}
