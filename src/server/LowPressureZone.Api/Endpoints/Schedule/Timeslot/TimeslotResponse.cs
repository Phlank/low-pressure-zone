using LowPressureZone.Api.Endpoints.Performer;
using LowPressureZone.Domain.Enums;

namespace LowPressureZone.Api.Endpoints.Schedule.Timeslot;

public class TimeslotResponse
{
    public required Guid Id { get; set; }
    public required PerformerResponse Performer { get; set; }
    public required PerformanceType PerformanceType { get; set; }
    public string? Name { get; set; }
    public required DateTime Start { get; set; }
    public required DateTime End { get; set; }
    public required DateTime CreatedDate { get; set; }
    public required DateTime ModifiedDate { get; set; }
}
