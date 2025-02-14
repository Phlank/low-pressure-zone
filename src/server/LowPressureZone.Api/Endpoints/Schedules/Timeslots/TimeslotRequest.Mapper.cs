using FastEndpoints;
using LowPressureZone.Domain.Entities;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class TimeslotRequestMapper : RequestMapper<TimeslotRequest, Timeslot>
{
    public override Timeslot ToEntity(TimeslotRequest r)
    {
        return new Timeslot
        {
            Id = Guid.NewGuid(),
            Name = r.Name?.Trim(),
            Start = r.Start.ToUniversalTime(),
            End = r.End.ToUniversalTime(),
            Type = r.PerformanceType.Trim(),
            PerformerId = r.PerformerId,
            ScheduleId = Guid.Empty,
            CreatedDate = DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow,
        };
    }
}
