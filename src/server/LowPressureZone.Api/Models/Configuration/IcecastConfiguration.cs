namespace LowPressureZone.Api.Models.Configuration;

public sealed class IcecastConfiguration
{
    public const string Name = "Icecast";
    public required Uri IcecastUrl { get; set; }
}
