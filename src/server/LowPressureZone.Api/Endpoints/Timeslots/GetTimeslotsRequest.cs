namespace LowPressureZone.Api.Endpoints.Timeslots;

public sealed class GetTimeslotsRequest
{
    public required Guid ScheduleId { get; set; }
}