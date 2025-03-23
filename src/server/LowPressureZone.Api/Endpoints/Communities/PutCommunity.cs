using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities;

public sealed class PutCommunity(DataContext dataContext) : EndpointWithMapper<CommunityRequest, CommunityMapper>
{
    public override void Configure()
    {
        Put("/communities/{id}");
        Description(builder => builder.Produces(404));
        Roles(RoleNames.Admin);
    }

    public override async Task HandleAsync(CommunityRequest request, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var community = await dataContext.Communities.FirstOrDefaultAsync(community => community.Id == id, ct);
        if (community == null || community.IsDeleted && !User.IsInRole(RoleNames.Admin))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await Map.UpdateEntityAsync(request, community, ct);
        await SendNoContentAsync(ct);
    }
}
