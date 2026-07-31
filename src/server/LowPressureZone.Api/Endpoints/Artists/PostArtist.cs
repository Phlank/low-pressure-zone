using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Endpoints.Artists;

public class PostArtist(DataContext dataContext) : Endpoint<ArtistRequest>
{
    public override void Configure()
    {
        Post("/artists");
        Roles(RoleNames.Admin);
    }

    public override async Task HandleAsync(ArtistRequest req, CancellationToken ct)
    {
        var artist = new Artist
        {
            Name = req.Name,
            PromoUrl = req.PromoUrl
        };
        dataContext.Artists.Add(artist);
        await dataContext.SaveChangesAsync(ct);
        await Send.CreatedAtAsync<GetArtist>(new { id = artist.Id }, cancellation: ct);
    }
}