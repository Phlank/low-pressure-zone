using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities.Relationships;

public class UpdateCommunityRelationship(DataContext dataContext, IdentityContext identityContext) : EndpointWithMapper<CommunityRelationshipRequest, CommunityRelationshipMapper>
{
    public override void Configure()
    {
        Verbs(Http.PUT, Http.POST);
        Routes("/communities/{communityId}/relationships/{userId}");
        Roles(RoleNames.Admin);
    }

    public override async Task HandleAsync(CommunityRelationshipRequest request, CancellationToken ct)
    {
        var communityId = Route<Guid>("communityId");
        var userId = Route<Guid>("userId");
        if (!await dataContext.Communities.AnyAsync(community => community.Id == communityId, ct)
            || !await identityContext.Users.AnyAsync(user => user.Id == userId, ct))
        {
            await SendNotFoundAsync(ct);
            return;
        }
        var existing = await dataContext.CommunityRelationships.FirstOrDefaultAsync(relationship => relationship.CommunityId == communityId && relationship.UserId == userId, ct);
        if (existing != null)
        {
            await Map.UpdateEntityAsync(request, existing, ct);
            await SendNoContentAsync(ct);
            return;
        }

        await dataContext.AddAsync(Map.ToEntity(request), ct);
        await dataContext.SaveChangesAsync(ct);
        await SendCreatedAtAsync<GetCommunityRelationshipById>(new
        {
            communityId,
            userId
        }, null, Http.GET, cancellation: ct);
    }
}
