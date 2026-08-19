using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Data;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities;

public class DeleteCommunity(DataContext dataContext, CommunityRules rules) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/communities/{id}");
        Description(builder => builder.Produces(204)
                                      .Produces(404));
        Roles(RoleNames.Admin);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var community = await dataContext.Communities
                                         .AsNoTracking()
                                         .Where(a => a.Id == id)
                                         .FirstOrDefaultAsync(ct);

        if (community is null || community.IsDeleted)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (!rules.IsDeleteAuthorized(community))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        this.ThrowIfDomainError(community.Delete());
        await dataContext.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}