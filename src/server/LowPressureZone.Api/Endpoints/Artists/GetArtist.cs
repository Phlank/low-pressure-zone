using FastEndpoints;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Artists;

public class GetArtist(DataContext dataContext) : EndpointWithoutRequest<ArtistResponse>
{
    public override void Configure()
    {
        Get("/artists/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var artist = await dataContext.Artists
                                      .FirstOrDefaultAsync(artist => artist.Id == id, ct);
        
        if (artist is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var response = new ArtistResponse
        {
            Id = artist.Id,
            Name = artist.Name,
            PromoUrl = artist.PromoUrl,
        };
        await Send.OkAsync(response, ct);
    }
}