using LowPressureZone.Api.Endpoints.Audience;
using LowPressureZone.Api.Endpoints.Schedule.Timeslot;

namespace LowPressureZone.Api.Endpoints.Schedule;

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
