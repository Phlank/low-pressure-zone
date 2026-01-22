using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Soundclashes;

public class DeleteSoundclash(DataContext dataContext, SoundclashRules rules) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/soundclashes/{id:guid}");
        Roles(RoleNames.Organizer, RoleNames.Admin);
        Description(builder => builder.Produces(404)
                                      .Produces(204)
                                      .WithTags("Soundclashes"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var soundclash = await dataContext.Soundclashes
                                          .GetSoundclashForUpdate(id, User.GetIdOrDefault())
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(ct);
        if (soundclash is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (!rules.IsDeleteAuthorized(soundclash))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        await dataContext.Soundclashes
                         .Where(item => item.Id == id)
                         .ExecuteDeleteAsync(ct);
        await Send.NoContentAsync(ct);
    }
}