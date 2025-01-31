using LowPressureZone.Api.Endpoints.Audience;

namespace LowPressureZone.Api.Endpoints.Schedule;

public class ScheduleResponse
{
    public required Guid Id { get; set; }
    public required DateTime Start { get; set; }
    public required DateTime End { get; set; }
    public required AudienceResponse Audience { get; set; }
}
