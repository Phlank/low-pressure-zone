using FastEndpoints;
using LowPressureZone.Api.Endpoints.Schedules;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Soundclashes;

public class PostSoundclash(DataContext dataContext, ScheduleRules scheduleRules)
    : EndpointWithMapper<SoundclashRequest, SoundclashMapper>
{
    public override void Configure()
    {
        Post("/soundclashes");
        Roles(RoleNames.Organizer, RoleNames.Admin);
        Description(builder => builder.Produces(201)
                                      .WithTags("Soundclashes"));
    }

    public override async Task HandleAsync(SoundclashRequest req, CancellationToken ct)
    {
        var schedule = await dataContext.Schedules
                                        .GetSchedulesForResponse(User.GetIdOrDefault())
                                        .Where(schedule => schedule.Id == req.ScheduleId)
                                        .FirstAsync(ct);

        if (!scheduleRules.IsAddingSoundclashesAllowed(schedule))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var soundclash = Map.FromRequest(req);
        dataContext.Soundclashes.Add(soundclash);
        await dataContext.SaveChangesAsync(ct);
        HttpContext.ExposeLocation();
        await SendCreatedAtAsync<GetSoundclashById>(new
        {
            soundclash.Id
        }, cancellation: ct);
    }
}