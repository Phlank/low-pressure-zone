using FastEndpoints;
using LowPressureZone.Api.Endpoints.Performers;
using LowPressureZone.Domain.Entities;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class TimeslotResponseMapper : ResponseMapper<TimeslotResponse, Timeslot>
{
    public required PerformerResponseMapper PerformerMapper { get; set; }

    public override TimeslotResponse FromEntity(Timeslot t)
    {
        return new TimeslotResponse
        {
            Id = t.Id,
            Start = t.Start,
            End = t.End,
            Name = t.Name,
            Performer = PerformerMapper.FromEntity(t.Performer!),
            PerformanceType = t.Type,
            CreatedDate = t.CreatedDate,
            ModifiedDate = t.LastModifiedDate
        };
    }
}
