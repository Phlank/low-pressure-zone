using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Soundclashes;

public class PutSoundclash(DataContext dataContext, SoundclashRules rules) : EndpointWithMapper<SoundclashRequest, SoundclashMapper>
{
    public override void Configure()
    {
        Put("/soundclashes/{id}");
        Description(builder => builder.Produces(404)
                                      .Produces(204)
                                      .WithTags("Soundclashes"));
        Roles(RoleNames.Admin, RoleNames.Organizer);
    }

    public override async Task HandleAsync(SoundclashRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var mapped = Map.FromRequest(req);

        var soundclash = await dataContext.Soundclashes
                                          .GetSoundclashForUpdate(id, User.GetIdOrDefault())
                                          .FirstOrDefaultAsync(ct);
        
        if (soundclash == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (!rules.IsEditAuthorized(soundclash))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        soundclash.PerformerOneId = mapped.PerformerOneId;
        soundclash.PerformerTwoId = mapped.PerformerTwoId;
        soundclash.RoundOne = mapped.RoundOne;
        soundclash.RoundTwo = mapped.RoundTwo;
        soundclash.RoundThree = mapped.RoundThree;
        soundclash.StartsAt = mapped.StartsAt;
        soundclash.EndsAt = mapped.EndsAt;
        soundclash.ScheduleId = mapped.ScheduleId;
        soundclash.LastModifiedDate = mapped.LastModifiedDate;

        await dataContext.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}