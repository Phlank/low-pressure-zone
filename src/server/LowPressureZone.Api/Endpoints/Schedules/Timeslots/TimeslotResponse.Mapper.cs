using FastEndpoints;
using LowPressureZone.Api.Endpoints.Performers;
using LowPressureZone.Domain.Entities;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class TimeslotResponseMapper : ResponseMapper<TimeslotResponse, Timeslot>
{
    private readonly PerformerResponseMapper _performerMapper;

    public TimeslotResponseMapper(PerformerResponseMapper performerMapper)
    {
        _performerMapper = performerMapper;
    }

    public override TimeslotResponse FromEntity(Timeslot t)
    {
        return new TimeslotResponse
        {
            Id = t.Id,
            Start = t.Start,
            End = t.End,
            Name = t.Name,
            Performer = _performerMapper.FromEntity(t.Performer!),
            PerformanceType = t.Type
        };
    }
}
