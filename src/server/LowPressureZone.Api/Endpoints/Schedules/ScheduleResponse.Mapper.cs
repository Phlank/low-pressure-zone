using FastEndpoints;
using LowPressureZone.Api.Endpoints.Audiences;
using LowPressureZone.Api.Endpoints.Schedules.Timeslots;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain;
using LowPressureZone.Domain.BusinessRules;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class ScheduleResponseMapper : ResponseMapper<ScheduleResponse, Domain.Entities.Schedule>
{
    private readonly ScheduleRules _rules;
    private readonly TimeslotResponseMapper _timeslotMapper;
    private readonly AudienceResponseMapper _audienceMapper;

    public ScheduleResponseMapper(ScheduleRules enforcer,
                                  TimeslotResponseMapper timeslotMapper,
                                  AudienceResponseMapper audienceMapper)
    {
        _rules = enforcer;
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
            Timeslots = s.Timeslots.Select(_timeslotMapper.FromEntity).ToList(),
            IsEditable = _rules.CanUserEditSchedule(s),
            IsDeletable = _rules.CanUserDeleteSchedule(s),
            IsTimeslotCreationAllowed = _rules.CanUserAddTimeslotsToSchedule(s)
        };
    }
}
