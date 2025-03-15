using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities;

public sealed class GetCommunities(DataContext dataContext)
    : EndpointWithoutRequest<IEnumerable<CommunityResponse>, CommunityMapper>
{
    public override void Configure()
    {
        Get("/communities");
        Description(builder => builder.Produces<List<CommunityResponse>>());
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var communitiesQuery = dataContext.Communities.Include(community => community.Relationships).AsNoTracking();
        if (!User.IsInRole(RoleNames.Admin)) communitiesQuery = communitiesQuery.Where(community => community.IsDeleted);

        var communities = await communitiesQuery.ToListAsync(ct);

        var responses = communities.Select(Map.FromEntity);
        await SendOkAsync(responses, ct);
    }
}
