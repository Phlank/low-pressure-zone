using FastEndpoints;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Soundclashes;

public class GetSoundclashById(DataContext dataContext) : EndpointWithoutRequest<SoundclashResponse, SoundclashMapper>
{
    public override void Configure()
    {
        Get("/soundclashes/{id}");
        AllowAnonymous();
        Description(builder => builder.Produces(404)
                                      .WithTags("Soundclashes"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var soundclash = await dataContext.Soundclashes
                                          .GetSoundclashesForResponse()
                                          .Where(soundclash => soundclash.Id == id)
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(ct);
        
        if (soundclash is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var response = Map.FromEntity(soundclash);
        await SendOkAsync(response, ct);
    }
}