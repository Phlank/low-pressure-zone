using LowPressureZone.Domain.Interfaces;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class TimeslotRequest : IDateTimeRange
{
    public required Guid PerformerId { get; set; }
    public required string PerformanceType { get; set; }
    public string? Name { get; set; }
    public required DateTime StartsAt { get; set; }
    public required DateTime EndsAt { get; set; }
}
