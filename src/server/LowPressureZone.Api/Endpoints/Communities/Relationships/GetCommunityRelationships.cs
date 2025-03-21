﻿using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities.Relationships;

public class GetCommunityRelationships(DataContext dataContext, IdentityContext identityContext) : EndpointWithoutRequest<IEnumerable<CommunityRelationshipResponse>, CommunityRelationshipMapper>
{
    public override void Configure() => Get("/communities/{communityId}/relationships");

    public override async Task HandleAsync(CancellationToken ct)
    {
        var communityId = Route<Guid>("communityId");

        var relationships = await dataContext.CommunityRelationships
                                             .AsSplitQuery()
                                             .Where(relationship => relationship.CommunityId == communityId)
                                             .Where(relationship => relationship.IsOrganizer || relationship.IsPerformer)
                                             .Include(relationship => relationship.Community)
                                             .ThenInclude(community => community.Relationships)
                                             .ToListAsync(ct);
        var relationshipUserIds = relationships.Select(relationship => relationship.UserId).Distinct();
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
                                     .Select(relationship => Map.FromEntity(relationship, displayNames[relationship.UserId]));
        await SendOkAsync(responses, ct);
    }
}
