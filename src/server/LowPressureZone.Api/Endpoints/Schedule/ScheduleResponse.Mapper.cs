using FastEndpoints;
using LowPressureZone.Api.Endpoints.Audiences;
using LowPressureZone.Api.Endpoints.Schedules.Timeslots;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class ScheduleResponseMapper : ResponseMapper<ScheduleResponse, Domain.Entities.Schedule>
{
    public required TimeslotResponseMapper TimeslotMapper { get; set; }
    public required AudienceResponseMapper AudienceMapper { get; set; }

    public override ScheduleResponse FromEntity(Domain.Entities.Schedule s)
    {
        return new ScheduleResponse
        {
            Id = s.Id,
            Start = s.Start,
            End = s.End,
            Audience = AudienceMapper.FromEntity(s.Audience!),
            Timeslots = s.Timeslots.Select(TimeslotMapper.FromEntity).ToList(),
            CreatedDate = s.CreatedDate,
            ModifiedDate = s.LastModifiedDate,
        };
    }
}
