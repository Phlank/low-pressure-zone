using FastEndpoints;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Soundclashes;

public class GetSoundclashes(DataContext dataContext)
    : Endpoint<GetSoundclashesRequest, IEnumerable<SoundclashResponse>, SoundclashMapper>
{
    public override void Configure()
    {
        Get("/soundclashes");
        AllowAnonymous();
        Description(builder => builder.Produces(200)
                                      .Produces(404)
                                      .WithTags("Soundclashes"));
    }

    public override async Task HandleAsync(GetSoundclashesRequest req, CancellationToken ct)
    {
        var doesScheduleExist = await dataContext.Schedules
                                                 .AnyAsync(schedule => schedule.Id == req.ScheduleId, ct);
        if (!doesScheduleExist)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var soundclashes = await dataContext.Soundclashes
                                            .GetSoundclashesForResponse()
                                            .Where(soundclash => soundclash.ScheduleId == req.ScheduleId)
                                            .ToListAsync(ct);
        var response = soundclashes.Select(Map.FromEntity);
        await Send.OkAsync(response, ct);
    }
}