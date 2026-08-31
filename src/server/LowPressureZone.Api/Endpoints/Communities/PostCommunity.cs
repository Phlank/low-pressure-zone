using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Data;
using LowPressureZone.Domain.CommunityAggregate;
using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Endpoints.Communities;

public sealed class PostCommunity(DataContext dataContext) : Endpoint<CommunityRequest>
{
    public override void Configure()
    {
        Post("/communities");
        Description(b => b.Produces(201));
        Roles(RoleNames.Admin);
    }

    public override async Task HandleAsync(CommunityRequest req, CancellationToken ct)
    {
        var result = Community.Create(req.Name, req.Url);
        await this.PublishOrThrowAsync(result);
        
        dataContext.Communities.Add(result.Value);
        await dataContext.SaveChangesAsync(ct);
        
        HttpContext.ExposeLocation();
        await Send.CreatedAtAsync<GetCommunities>(new
        {
            result.Value.Id
        }, Response, cancellation: ct);
    }
}