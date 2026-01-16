using LowPressureZone.Domain.Interfaces;

namespace LowPressureZone.Api.Endpoints.Soundclashes;

public class SoundclashRequest : IDateTimeRange
{
    public required Guid ScheduleId { get; set; }
    public required Guid PerformerOneId { get; set; }
    public required Guid PerformerTwoId { get; set; }
    public required string RoundOne { get; set; }
    public required string RoundTwo { get; set; }
    public required string RoundThree { get; set; }
    public required DateTimeOffset StartsAt { get; set; }
    public required DateTimeOffset EndsAt { get; set; }
}