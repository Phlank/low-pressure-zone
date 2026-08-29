using FastEndpoints;
using LowPressureZone.Data;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.ClashSlots;

public class GetClashSlotById(DataContext dataContext) : EndpointWithoutRequest<ClashSlotResponse, ClashSlotMapper>
{
    public override void Configure()
    {
        Get("/schedules/{scheduleId}/clash-slots/{id}");
        Description(builder => builder.Produces(404));
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var id = Route<Guid>("id");

        var schedule = await dataContext.NewSchedules.FirstOrDefaultAsync(schedule => schedule.Id == scheduleId, ct);
        var slot = schedule?.ClashSlots.FirstOrDefault(slot => slot.Id == id);
        if (schedule is null || slot is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }
        
        await Send.OkAsync(Map.FromEntity(slot), ct);
    }
}