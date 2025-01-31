using LowPressureZone.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Domain.QueryableExtensions;

public static class ScheduleQueryableExtensions
{
    public static IQueryable<Schedule> IncludeConnectingProperties(this IQueryable<Schedule> query)
    {
        return query.Include(nameof(Schedule.Audience))
                    .Include(nameof(Schedule.Timeslots))
                    .Include($"{nameof(Schedule.Timeslots)}.{nameof(Timeslot.Performer)}");
    }
}
