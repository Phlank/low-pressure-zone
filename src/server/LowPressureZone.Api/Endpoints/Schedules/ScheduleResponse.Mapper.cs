using FastEndpoints;
using LowPressureZone.Api.Endpoints.Audiences;
using LowPressureZone.Api.Endpoints.Schedules.Timeslots;
using LowPressureZone.Domain.BusinessRules;
using LowPressureZone.Domain.Entities;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class ScheduleResponseMapper : ResponseMapper<ScheduleResponse, Schedule>
{
    private readonly ScheduleRules _rules;
    private readonly TimeslotResponseMapper _timeslotMapper;
    private readonly AudienceMapper _audienceMapper;

    public ScheduleResponseMapper(ScheduleRules rules,
                                  TimeslotResponseMapper timeslotMapper,
                                  AudienceMapper audienceMapper)
    {
        _rules = rules;
        _timeslotMapper = timeslotMapper;
        _audienceMapper = audienceMapper;
    }

    public override ScheduleResponse FromEntity(Schedule s)
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
