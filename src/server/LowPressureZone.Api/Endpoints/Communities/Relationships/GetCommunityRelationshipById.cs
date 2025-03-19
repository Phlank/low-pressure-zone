using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Identity;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Communities.Relationships;

public class GetCommunityRelationshipById(DataContext dataContext, IdentityContext identityContext) : EndpointWithoutRequest<CommunityRelationshipResponse, CommunityRelationshipMapper>
{
    public override void Configure()
    {
        Get("/communities/{communityId}/relationships/{userId}");
        Description(builder => builder.Produces(404));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var communityId = Route<Guid>("communityId");
        var userId = Route<Guid>("userId");
        var relationship = await dataContext.CommunityRelationships
                                            .AsNoTracking()
                                            .Where(relationship => relationship.CommunityId == communityId && relationship.UserId == userId)
                                            .Where(relationship => relationship.IsOrganizer || relationship.IsPerformer)
                                            .FirstOrDefaultAsync(ct);
        if (relationship == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        var displayName = await identityContext.Users
                                               .AsNoTracking()
                                               .Where(user => user.Id == userId)
                                               .Select(user => user.DisplayName)
                                               .FirstOrDefaultAsync(ct);
        displayName.ShouldNotBeNull();
        await SendOkAsync(Map.FromEntity(relationship, displayName), ct);
    }
}
