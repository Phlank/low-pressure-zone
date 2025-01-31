using FastEndpoints;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class TimeslotRequestMapper : RequestMapper<TimeslotRequest, Domain.Entities.Timeslot>
{
    public override Domain.Entities.Timeslot ToEntity(TimeslotRequest r)
    {
        return new Domain.Entities.Timeslot
        {
            Id = Guid.NewGuid(),
            Name = r.Name,
            Start = r.Start,
            End = r.End,
            Type = r.PerformanceType,
            PerformerId = r.PerformerId,
            ScheduleId = Guid.Empty,
            CreatedDate = DateTime.Now,
            LastModifiedDate = DateTime.Now,
        };
    }
}
