using FastEndpoints;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities;

public sealed class GetCommunityById(DataContext dataContext)
    : EndpointWithoutRequest<CommunityResponse, CommunityMapper>
{
    public override void Configure()
    {
        Get("/communities/{id}");
        Description(builder => builder.Produces<CommunityResponse>(200).Produces(404));
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var community = await dataContext.Communities.AsNoTracking()
            .FirstOrDefaultAsync(community => community.Id == id && !community.IsDeleted, ct);
        if (community == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var response = await Map.FromEntityAsync(community, ct);
        await SendOkAsync(response, ct);
    }
}