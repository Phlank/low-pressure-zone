namespace LowPressureZone.Api.Endpoints.Schedule;

public sealed class ScheduleRequest
{
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    public required Guid AudienceId { get; set; }
}
