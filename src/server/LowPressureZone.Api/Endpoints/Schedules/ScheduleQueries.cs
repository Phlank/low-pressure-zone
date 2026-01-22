using LowPressureZone.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules;

public static class ScheduleQueries
{
    public static IQueryable<Schedule> GetSchedulesForResponse(this IQueryable<Schedule> queryable, Guid userId) =>
        queryable.OrderBy(schedule => schedule.StartsAt)
                 .Include(schedule => schedule.Community)
                 .ThenInclude(community => community.Relationships
                                                    .Where(relationship => relationship.UserId == userId))
                 .Include(schedule => schedule.Timeslots
                                              .OrderBy(timeslot => timeslot.StartsAt))
                 .ThenInclude(timeslot => timeslot.Performer)
                 .Include(schedule => schedule.Soundclashes
                                              .OrderBy(soundclash => soundclash.StartsAt))
                 .ThenInclude(soundclash => soundclash.PerformerOne)
                 .Include(schedule => schedule.Soundclashes
                                              .OrderBy(soundclash => soundclash.StartsAt))
                 .ThenInclude(soundclash => soundclash.PerformerTwo)
                 .AsNoTracking()
                 .AsSplitQuery();
}