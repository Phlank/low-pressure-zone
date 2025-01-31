using LowPressureZone.Api.Endpoints.Audiences;
using LowPressureZone.Api.Endpoints.Schedules.Timeslots;

namespace LowPressureZone.Api.Endpoints.Schedules;

public sealed class ScheduleResponse
{
    public required Guid Id { get; set; }
    public required DateTime Start { get; set; }
    public required DateTime End { get; set; }
    public required AudienceResponse Audience { get; set; }
    public required List<TimeslotResponse> Timeslots { get; set; }
    public required DateTime CreatedDate { get; set; }
    public required DateTime ModifiedDate { get; set; }
}
