using LowPressureZone.Domain.Interfaces;

namespace LowPressureZone.Api.Endpoints.Schedules;

public sealed class ScheduleRequest : IDateTimeRange
{
    public required DateTime StartsAt { get; set; }
    public required DateTime EndsAt { get; set; }
    public string Description { get; set; } = string.Empty;
    public required Guid AudienceId { get; set; }
}
