using FastEndpoints;
using LowPressureZone.Api.Endpoints.Performer;

namespace LowPressureZone.Api.Endpoints.Schedule.Timeslot;

public class TimeslotResponseMapper : ResponseMapper<TimeslotResponse, Domain.Entities.Timeslot>
{
    public required PerformerResponseMapper PerformerMapper { get; set; }

    public override TimeslotResponse FromEntity(Domain.Entities.Timeslot t)
    {
        return new TimeslotResponse
        {
            Id = t.Id,
            Start = t.Start,
            End = t.End,
            Name = t.Name,
            Performer = PerformerMapper.FromEntity(t.Performer),
            PerformanceType = t.Type,
            CreatedDate = t.CreatedDate,
            ModifiedDate = t.LastModifiedDate
        };
    }
}
