using LowPressureZone.Domain.Enums;

namespace LowPressureZone.Domain.Entities;

public class ReleaseCredit : BaseEntity
{
    public required Guid ArtistId { get; set; }
    public Artist? Artist { get; set; }
    public required Guid ReleaseId { get; set; }
    public Release? Release { get; set; }
    public ReleaseContribution Type { get; set; }
}