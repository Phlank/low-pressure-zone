namespace LowPressureZone.Domain.Entities;

public sealed class CommunityRelationship : BaseEntity
{
    public required Guid CommunityId { get; set; }
    public Community Community { get; set; } = null!;
    public required Guid UserId { get; set; }
    public required bool IsOrganizer { get; set; }
    public required bool IsPerformer { get; set; }
}