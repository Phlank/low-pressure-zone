using LowPressureZone.Domain.Interfaces;

namespace LowPressureZone.Api.Endpoints.Schedules;

public sealed class ScheduleRequest : IDateTimeRange
{
    public required DateTime Start { get; set; }
    public required DateTime End { get; set; }
    public required Guid AudienceId { get; set; }
}
