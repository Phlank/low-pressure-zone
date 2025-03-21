using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities;

public sealed class GetCommunities(DataContext dataContext)
    : EndpointWithoutRequest<IEnumerable<CommunityResponse>, CommunityMapper>
{
    public override void Configure() => Get("/communities");

    public override async Task HandleAsync(CancellationToken ct)
    {
        IQueryable<Community> communitiesQuery = dataContext.Communities
                                                            .AsNoTracking()
                                                            .Include(community => community.Relationships.Where(relationship => relationship.UserId == User.GetIdOrDefault()));
        if (!User.IsInRole(RoleNames.Admin))
            communitiesQuery = communitiesQuery.Where(community => !community.IsDeleted);

        var communities = await communitiesQuery.ToListAsync(ct);

        var responses = communities.Select(Map.FromEntity);
        await SendOkAsync(responses, ct);
    }
}
