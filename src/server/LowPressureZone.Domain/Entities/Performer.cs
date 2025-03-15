namespace LowPressureZone.Domain.Entities;

public sealed class Performer : BaseEntity
{
    public required string Name { get; set; }
    public required string Url { get; set; }
    public required List<Guid> LinkedUserIds { get; set; } = [];

    public List<Timeslot> Timeslots { get; set; } = [];
    public bool IsDeleted { get; set; }
}
