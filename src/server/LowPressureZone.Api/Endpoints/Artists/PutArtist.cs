using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Artists;

public class PutArtist(DataContext dataContext) : Endpoint<ArtistRequest>
{
    public override void Configure()
    {
        Put("/artists/{id}");
        Roles(RoleNames.Admin, RoleNames.Organizer);
    }

    public override async Task HandleAsync(ArtistRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var artist = await dataContext.Artists
                                      .Where(artist => artist.Id == id)
                                      .FirstOrDefaultAsync(ct);
        
        if (artist is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        artist.Name = req.Name;
        artist.PromoUrl = req.PromoUrl;
        await dataContext.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}