using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities.Relationships;

public class PostCommunityRelationship(DataContext dataContext, IdentityContext identityContext) : EndpointWithMapper<CommunityRelationshipRequest, CommunityRelationshipMapper>
{
    public override void Configure()
    {
        Post("/communities/{communityId}/relationships/{userId}");
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
        var relationship = Map.ToEntity(request);
        await dataContext.AddAsync(relationship, ct);
        await dataContext.SaveChangesAsync(ct);
    }
}
