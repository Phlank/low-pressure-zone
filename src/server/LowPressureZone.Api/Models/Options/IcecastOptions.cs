namespace LowPressureZone.Api.Models.Options;

public class IcecastOptions
{
    public const string Name = "Icecast";
    public required Uri IcecastUrl { get; set; }
}
