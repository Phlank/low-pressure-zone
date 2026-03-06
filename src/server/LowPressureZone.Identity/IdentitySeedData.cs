namespace LowPressureZone.Identity;

public sealed class IdentitySeedData
{
    public required string AdminDisplayName { get; set; }
    public required string AdminUsername { get; set; }
    public required string AdminEmail { get; set; }
    public required string AdminPassword { get; set; }
}