using FastEndpoints;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Rules;
using LowPressureZone.Api.Services.Files;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Timeslots;

public class PutTimeslot(DataContext dataContext, PrerecordedMixFileProcessor fileProcessor, TimeslotRules rules)
    : EndpointWithMapper<TimeslotRequest, TimeslotMapper>
{
    public override void Configure()
    {
        Put("/timeslots/{id}");
        AllowFormData();
        AllowFileUploads();
        Description(builder => builder.Produces(204)
                                      .Produces(404));
    }

    public override async Task HandleAsync(TimeslotRequest request, CancellationToken ct)
    {
        var timeslotId = Route<Guid>("id");

        var timeslot = await dataContext.Timeslots
                                        .Include(timeslot => timeslot.Performer)
                                        .Include(timeslot => timeslot.Schedule)
                                        .Where(timeslot => timeslot.Id == timeslotId)
                                        .FirstOrDefaultAsync(ct);

        if (timeslot == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (!rules.IsEditAuthorized(timeslot))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        if (request.PerformanceType == PerformanceTypes.Prerecorded
            && timeslot.Type == PerformanceTypes.Prerecorded)
        {
            var updateAzuraCastResult = await fileProcessor.UpdateEnqueuedPrerecordedMixAsync(timeslotId, request, ct);
            if (updateAzuraCastResult.IsError)
                ValidationFailures.AddRange(updateAzuraCastResult.Error);

            ThrowIfAnyErrors();
            timeslot.AzuraCastMediaId = updateAzuraCastResult.Value;
        }

        Map.UpdateEntity(request, timeslot);
        _ = await dataContext.SaveChangesAsync(ct);

        await SendNoContentAsync(ct);
    }
}