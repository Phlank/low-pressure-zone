namespace LowPressureZone.Domain.Entities;

public sealed class Community : BaseEntity
{
    public required string Name { get; set; }
    public required string Url { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<Schedule> Schedules { get; } = [];
    public ICollection<CommunityRelationship> Relationships { get; } = [];
}
