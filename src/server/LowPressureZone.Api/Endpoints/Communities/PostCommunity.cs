using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Endpoints.Communities;

public sealed class PostCommunity(DataContext dataContext) : EndpointWithMapper<CommunityRequest, CommunityMapper>
{
    public override void Configure()
    {
        Post("/communities");
        Description(b => b.Produces(201));
        Roles(RoleNames.Admin);
    }

    public override async Task HandleAsync(CommunityRequest req, CancellationToken ct)
    {
        var community = Map.ToEntity(req);
        dataContext.Communities.Add(community);
        await dataContext.SaveChangesAsync(ct);
        await SendCreatedAtAsync<GetCommunities>(new
        {
            community.Id
        }, Response, cancellation: ct);
    }
}
