using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities;

public sealed class GetCommunities(DataContext dataContext, CommunityRules rules)
    : EndpointWithoutRequest<IEnumerable<CommunityResponse>, CommunityMapper>
{
    public override void Configure() => Get("/communities");

    public override async Task HandleAsync(CancellationToken ct)
    {
        var communities = await dataContext.Communities
                                           .AsNoTracking()
                                           .Include(community => community.Relationships.Where(relationship => relationship.UserId == User.GetIdOrDefault()))
                                           .OrderBy(community => community.Name)
                                           .ToListAsync(ct);

        communities.RemoveAll(rules.IsHiddenFromApi);

        var responses = communities.Select(Map.FromEntity);
        await SendOkAsync(responses, ct);
    }
}
