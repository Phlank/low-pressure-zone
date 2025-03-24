using System.ComponentModel.DataAnnotations;

namespace LowPressureZone.Domain.Entities;

public sealed class Performer : BaseEntity
{
    [MaxLength(64)]
    public required string Name { get; set; }

    [MaxLength(256)]
    public required string? Url { get; set; }

    public required ICollection<Guid> LinkedUserIds { get; set; } = [];

    public ICollection<Timeslot> Timeslots { get; set; } = [];
    public bool IsDeleted { get; set; }
}
