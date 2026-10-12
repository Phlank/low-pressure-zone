using LowPressureZone.Domain.ScheduleAggregate;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules;

public static class ScheduleQueries
{
    public static IQueryable<Schedule> GetSchedulesForResponse(this IQueryable<Schedule> queryable, Guid userId) =>
        queryable.OrderBy(schedule => schedule.TimeRange.StartsAt)
                 .Include(schedule => schedule.Community)
                 .ThenInclude(community => community.Relationships
                                                    .Where(relationship => relationship.UserId == userId))
                 .Include(schedule => schedule.HourlySlots
                                              .OrderBy(slot => slot.TimeRange.StartsAt))
                 .ThenInclude(timeslot => timeslot.Performer)
                 .Include(schedule => schedule.ClashSlots
                                              .OrderBy(slot => slot.TimeRange.StartsAt))
                 .ThenInclude(soundclash => soundclash.PerformerOne)
                 .Include(schedule => schedule.ClashSlots
                                              .OrderBy(slot => slot.TimeRange.StartsAt))
                 .ThenInclude(soundclash => soundclash.PerformerTwo)
                 .AsNoTracking()
                 .AsSplitQuery();
}