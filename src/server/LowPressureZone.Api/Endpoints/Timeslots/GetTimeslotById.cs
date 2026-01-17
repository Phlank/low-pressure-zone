using FastEndpoints;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Timeslots;

public class GetTimeslotById : EndpointWithoutRequest<TimeslotResponse, TimeslotMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Get("/timeslots/{id}");
        Description(builder => builder.Produces(404));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var timeslotId = Route<Guid>("id");

        var timeslot = await DataContext.Timeslots
                                        .AsNoTracking()
                                        .Include(timeslot => timeslot.Performer)
                                        .Where(timeslot => timeslot.Id == timeslotId)
                                        .FirstOrDefaultAsync(ct);
        if (timeslot == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var response = Map.FromEntity(timeslot);
        await Send.OkAsync(response, ct);
    }
}