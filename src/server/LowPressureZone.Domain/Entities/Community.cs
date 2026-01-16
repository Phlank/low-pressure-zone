using System.ComponentModel.DataAnnotations;

namespace LowPressureZone.Domain.Entities;

public sealed class Community : BaseEntity
{
    [MaxLength(64)]
    public required string Name { get; set; }

    [MaxLength(256)]
    public required string Url { get; set; }

    public bool IsDeleted { get; set; }
    public ICollection<Schedule> Schedules { get; init; } = [];
    public ICollection<CommunityRelationship> Relationships { get; init; } = [];
}