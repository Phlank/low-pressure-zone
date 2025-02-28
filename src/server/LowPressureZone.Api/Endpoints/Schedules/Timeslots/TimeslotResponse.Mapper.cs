using FastEndpoints;
using LowPressureZone.Api.Endpoints.Performers;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain.BusinessRules;
using LowPressureZone.Domain.Entities;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class TimeslotResponseMapper : ResponseMapper<TimeslotResponse, Timeslot>
{
    private readonly TimeslotRules _rules;
    private readonly PerformerResponseMapper _performerMapper;

    public TimeslotResponseMapper(TimeslotRules rules,
                                  PerformerResponseMapper performerMapper)
    {
        _rules = rules;
        _performerMapper = performerMapper;
    }

    public override TimeslotResponse FromEntity(Timeslot timeslot)
    {
        var isEditable = _rules.CanUserEditTimeslot(timeslot);
        var isDeletable = _rules.CanUserDeleteTimeslot(timeslot);

        return new TimeslotResponse
        {
            Id = timeslot.Id,
            Start = timeslot.Start,
            End = timeslot.End,
            Name = timeslot.Name,
            Performer = _performerMapper.FromEntity(timeslot.Performer!),
            PerformanceType = timeslot.Type,
            IsEditable = isEditable,
            IsDeletable = isDeletable
        };
    }
}
