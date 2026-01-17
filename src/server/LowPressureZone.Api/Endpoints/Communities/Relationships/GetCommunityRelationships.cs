using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities.Relationships;

public class GetCommunityRelationships(DataContext dataContext, IdentityContext identityContext)
    : EndpointWithoutRequest<IEnumerable<CommunityRelationshipResponse>, CommunityRelationshipMapper>
{
    public override void Configure() => Get("/communities/{communityId}/relationships");

    public override async Task HandleAsync(CancellationToken ct)
    {
        var communityId = Route<Guid>("communityId");

        var relationships = await dataContext.CommunityRelationships
                                             .AsNoTracking()
                                             .Where(relationship => relationship.CommunityId == communityId)
                                             .ToListAsync(ct);
        var userRelationship =
            relationships.FirstOrDefault(relationship => relationship.UserId == User.GetIdOrDefault());
        var relationshipUserIds = relationships.Select(relationship => relationship.UserId);
        var displayNames = await identityContext.Users
                                                .AsNoTracking()
                                                .Where(user => relationshipUserIds.Contains(user.Id))
                                                .Select(user => new
                                                {
                                                    user.Id,
                                                    user.DisplayName
                                                })
                                                .ToDictionaryAsync(user => user.Id, user => user.DisplayName, ct);
        var responses = relationships.Where(relationship => displayNames.ContainsKey(relationship.UserId))
                                     .Select(relationship =>
                                                 Map.FromEntity(relationship, displayNames[relationship.UserId],
                                                                userRelationship))
                                     .OrderBy(response => response.DisplayName);
        await Send.OkAsync(responses, ct);
    }
}