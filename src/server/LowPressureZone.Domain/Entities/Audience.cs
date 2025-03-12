namespace LowPressureZone.Domain.Entities;

public class Audience : BaseEntity
{
    public required string Name { get; set; }
    public required string Url { get; set; }
    public bool IsDeleted { get; set; }
    public virtual List<Schedule>? Schedules { get; set; }
    public virtual List<AudienceRelationship>? Relationships { get; set; }
}