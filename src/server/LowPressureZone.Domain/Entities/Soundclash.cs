using LowPressureZone.Domain.Interfaces;

namespace LowPressureZone.Domain.Entities;

public sealed class Soundclash : BaseEntity, IDateTimeRange
{
    public required Guid ScheduleId { get; set; }
    public Schedule Schedule { get; set; } = null!;
    public required Guid PerformerOneId { get; set; }
    public Performer PerformerOne { get; set; } = null!;
    public required Guid PerformerTwoId { get; set; }
    public Performer PerformerTwo { get; set; } = null!;
    public required string RoundOne { get; set; }
    public required string RoundTwo { get; set; }
    public required string RoundThree { get; set; }
    public required DateTimeOffset StartsAt { get; set; }
    public required DateTimeOffset EndsAt { get; set; }
}