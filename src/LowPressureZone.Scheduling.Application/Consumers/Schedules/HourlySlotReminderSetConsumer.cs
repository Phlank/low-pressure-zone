using FastEndpoints;
using Hangfire;
using LowPressureZone.Data;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.Events;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Scheduling.Application.Consumers.Schedules;

public class HourlySlotReminderSetConsumer(
    IBackgroundJobClient backgroundJobClient,
    DataContext dataContext,
    UserManager<AppUser> userManager,
    ) : IEventHandler<HourlySlotReminderSet>
{
    public async Task HandleAsync(HourlySlotReminderSet eventModel, CancellationToken ct)
    {
        if (eventModel.OldJobId is not null)
        {
            backgroundJobClient.Delete(eventModel.OldJobId);
        }

        var slot = await dataContext.HourlySlots
                                    .Where(slot => slot.Id == eventModel.HourlySlotId)
                                    .Include(slot => slot.Performer)
                                    .FirstOrDefaultAsync(ct);
        if (slot is null) return;
        
        var user = await userManager.FindByIdAsync(slot.Performer.CreatorUserId.ToString());
        if (user is null) return;
        
        
    }
}