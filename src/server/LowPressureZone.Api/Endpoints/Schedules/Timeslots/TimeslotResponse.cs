using LowPressureZone.Api.Endpoints.Performers;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class TimeslotResponse
{
    public required Guid Id { get; set; }
    public required PerformerResponse Performer { get; set; }
    public required string PerformanceType { get; set; }
    public string? Name { get; set; }
    public required DateTime Start { get; set; }
    public required DateTime End { get; set; }
}
