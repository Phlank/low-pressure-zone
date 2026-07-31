using System.ComponentModel.DataAnnotations;
using LowPressureZone.Domain.Enums;

namespace LowPressureZone.Domain.Entities;

public class Release : BaseEntity
{
    [MaxLength(512)]
    public required string Name { get; set; }
    public required byte[] Artwork { get; set; }
    public required DateTime ReleaseDate { get; set; }
    [MaxLength(512)]
    public string? PromoUrl { get; set; }
    [MaxLength(512)]
    public string? PurchaseUrl { get; set; }
    public required ReleaseType Type { get; set; }
    [MaxLength(512)]
    public required string Label { get; set; }
    public List<ReleaseCredit> Credits { get; set; } = [];
}