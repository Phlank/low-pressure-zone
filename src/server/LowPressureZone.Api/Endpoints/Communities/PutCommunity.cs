using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Api.Constants;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities;

public sealed class PutCommunity(DataContext dataContext) : EndpointWithMapper<CommunityRequest, CommunityMapper>
{
    public override void Configure()
    {
        Put("/communities/{id}");
        Description(b => b.Produces(204)
            .Produces(404));
        AllowAnonymous();
    }

    public override async Task HandleAsync(CommunityRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var community = await dataContext.Communities.FirstOrDefaultAsync(a => a.Id == id, ct);
        if (community == null || (community.IsDeleted && !User.IsInRole(RoleNames.Admin)))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await Map.UpdateEntityAsync(req, community, ct);
        await SendNoContentAsync(ct);
    }
}