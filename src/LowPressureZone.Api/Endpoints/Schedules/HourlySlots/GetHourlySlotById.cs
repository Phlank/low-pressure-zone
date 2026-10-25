using FastEndpoints;
using LowPressureZone.Data;
using LowPressureZone.Identity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.HourlySlots;

public class GetHourlySlotById(DataContext dataContext) : EndpointWithoutRequest<HourlySlotResponse, HourlySlotMapper>
{
    public override void Configure()
    {
        Get("/schedules/{scheduleId}/hourlySlots/{id}");
        Description(builder => builder.Produces(404));
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var id = Route<Guid>("id");
        var slot = await dataContext.HourlySlots
                                    .IgnoreAutoIncludes()
                                    .Include(slot => slot.Performer)
                                    .Include(slot => slot.Schedule)
                                    .ThenInclude(schedule => schedule.Community)
                                    .ThenInclude(community => community.Relationships
                                                                       .Where(relationship =>
                                                                                  relationship.UserId ==
                                                                                  User.GetIdOrDefault()))
                                    .Where(slot => slot.Id == id && slot.ScheduleId == scheduleId)
                                    .FirstOrDefaultAsync(ct);

        if (slot is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(Map.FromEntity(slot), ct);
    }
}