namespace LowPressureZone.Domain.Entities;

public class AudienceRelationship : BaseEntity
{
    public required Guid AudienceId { get; set; }
    public virtual Audience? Audience { get; set; }
    public required Guid UserId { get; set; }
    public required bool IsOrganizer { get; set; }
    public required bool IsPerformer { get; set; }
}
