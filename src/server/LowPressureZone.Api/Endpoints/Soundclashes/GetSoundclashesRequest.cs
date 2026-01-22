namespace LowPressureZone.Api.Endpoints.Soundclashes;

public class GetSoundclashesRequest
{
    public required Guid ScheduleId { get; init; }
}