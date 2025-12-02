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
        Description(builder => builder.Produces(404));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var community = await dataContext.Communities
                                         .AsNoTracking()
                                         .Where(community => community.Id == id && !community.IsDeleted)
                                         .FirstOrDefaultAsync(ct);
        if (community == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var response = Map.FromEntity(community);
        await SendOkAsync(response, ct);
    }
}