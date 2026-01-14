using LowPressureZone.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules;

public static class ScheduleQueries
{
    public static IQueryable<Schedule> GetSchedulesForResponse(this IQueryable<Schedule> queryable, Guid userId) =>
        queryable.Include(schedule => schedule.Community)
                 .ThenInclude(community => community.Relationships
                                                    .Where(relationship => relationship.UserId == userId))
                 .Include(schedule => schedule.Timeslots)
                 .ThenInclude(timeslot => timeslot.Performer)
                 .Include(schedule => schedule.Soundclashes)
                 .ThenInclude(soundclash => soundclash.PerformerOne)
                 .Include(schedule => schedule.Soundclashes)
                 .ThenInclude(soundclash => soundclash.PerformerTwo)
                 .AsNoTracking()
                 .AsSplitQuery();
}