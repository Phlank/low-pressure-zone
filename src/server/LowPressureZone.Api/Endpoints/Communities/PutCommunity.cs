using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Core.Domain;
using LowPressureZone.Data;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities;

public sealed class PutCommunity(DataContext dataContext) : Endpoint<CommunityRequest>
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

        if (community is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var result = DomainResult.Compose(community.Rename(request.Name),
                                          community.ChangeSocialUrl(request.Url));

        this.ThrowIfDomainError(result);
        await dataContext.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}