using FastEndpoints;
using LowPressureZone.Data;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.HourlySlots;

public class GetHourlySlotById(DataContext dataContext) : EndpointWithoutRequest<HourlySlotResponse, HourlySlotMapper>
{
    public override void Configure()
    {
        Get("/schedules/{scheduleId}/hourly-slots/{id}");
        Description(builder => builder.Produces(404));
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var id = Route<Guid>("id");
        var slot = await dataContext.HourlySlots
                                    .Where(slot => slot.Id == id && slot.ScheduleId == scheduleId)
                                    .FirstOrDefaultAsync(ct);

        if (slot is null)
        {
            await Send.NoContentAsync(ct);
            return;
        }

        await Send.OkAsync(Map.FromEntity(slot), ct);
    }
}