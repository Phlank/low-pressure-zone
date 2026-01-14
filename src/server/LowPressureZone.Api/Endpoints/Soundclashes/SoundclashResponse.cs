using LowPressureZone.Api.Endpoints.Performers;

namespace LowPressureZone.Api.Endpoints.Soundclashes;

public sealed class SoundclashResponse
{
    public required Guid Id { get; set; }
    public required Guid ScheduleId { get; set; }
    public required PerformerResponse PerformerOne { get; set; }
    public required PerformerResponse PerformerTwo { get; set; }
    public required string RoundOne { get; set; }
    public required string RoundTwo { get; set; }
    public required string RoundThree { get; set; }
    public required DateTimeOffset StartsAt { get; set; }
    public required DateTimeOffset EndsAt { get; set; }
    public required bool IsEditable { get; set; }
    public required bool IsDeletable { get; set; }
}