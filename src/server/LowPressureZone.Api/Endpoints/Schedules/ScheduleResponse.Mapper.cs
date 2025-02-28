using FastEndpoints;
using LowPressureZone.Api.Endpoints.Audiences;
using LowPressureZone.Api.Endpoints.Schedules.Timeslots;
using LowPressureZone.Domain.BusinessRules;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class ScheduleResponseMapper : ResponseMapper<ScheduleResponse, Domain.Entities.Schedule>
{
    private readonly ScheduleRules _rules;
    private readonly TimeslotResponseMapper _timeslotMapper;
    private readonly AudienceResponseMapper _audienceMapper;

    public ScheduleResponseMapper(ScheduleRules rules,
                                  TimeslotResponseMapper timeslotMapper,
                                  AudienceResponseMapper audienceMapper)
    {
        _rules = rules;
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
            Description = s.Description,
            Audience = _audienceMapper.FromEntity(s.Audience!),
            Timeslots = s.Timeslots.OrderBy(t => t.Start).Select(_timeslotMapper.FromEntity),
            IsEditable = _rules.CanUserEditSchedule(s),
            IsDeletable = _rules.CanUserDeleteSchedule(s),
            IsTimeslotCreationAllowed = _rules.CanUserAddTimeslotsToSchedule(s)
        };
    }
}
