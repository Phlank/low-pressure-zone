using System.ComponentModel.DataAnnotations;

namespace LowPressureZone.Domain.Entities;

public class Artist : BaseEntity
{
    [MaxLength(512)]
    public required string Name { get; set; }

    [MaxLength(512)]
    public string? PromoUrl { get; set; }

    public List<ReleaseCredit> Credits { get; init; } = [];
}