namespace LowPressureZone.Api.Models.Options;

public sealed class UrlOptions
{
    public const string Name = "Urls";
    
    public required Uri SiteUrl { get; init; }
    public required Uri RegisterUrl { get; init; }
    public required Uri ResetPasswordUrl { get; init; }
}
