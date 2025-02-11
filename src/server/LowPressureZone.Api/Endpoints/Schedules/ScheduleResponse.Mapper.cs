using FastEndpoints;
using LowPressureZone.Api.Endpoints.Audiences;
using LowPressureZone.Api.Endpoints.Schedules.Timeslots;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class ScheduleResponseMapper : ResponseMapper<ScheduleResponse, Domain.Entities.Schedule>
{
    private readonly TimeslotResponseMapper _timeslotMapper;
    private readonly AudienceResponseMapper _audienceMapper;

    public ScheduleResponseMapper(TimeslotResponseMapper timeslotMapper, AudienceResponseMapper audienceMapper)
    {
        _timeslotMapper = timeslotMapper;
        _audienceMapper = audienceMapper;
    }

    public override ScheduleResponse FromEntity(Domain.Entities.Schedule s)
    {
        return new ScheduleResponse
        {
            Id = s.Id,
            Start = s.Start,
            End = s.End,
            Audience = _audienceMapper.FromEntity(s.Audience!),
            Timeslots = s.Timeslots.Select(_timeslotMapper.FromEntity).ToList(),
            CreatedDate = s.CreatedDate,
            ModifiedDate = s.LastModifiedDate,
        };
    }
}
