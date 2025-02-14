using FastEndpoints;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class ScheduleRequestMapper : RequestMapper<ScheduleRequest, Domain.Entities.Schedule>
{
    public override Domain.Entities.Schedule ToEntity(ScheduleRequest r)
    {
        return new Domain.Entities.Schedule
        {
            Id = Guid.NewGuid(),
            AudienceId = r.AudienceId,
            Start = r.Start.ToUniversalTime(),
            End = r.End.ToUniversalTime()
        };
    }
}
