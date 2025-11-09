namespace LowPressureZone.Api.Models.Configuration;

public sealed class UrlConfiguration
{
    public const string Name = "Urls";

    public required Uri SiteUrl { get; init; }
    public required Uri RegisterUrl { get; init; }
    public required Uri ResetPasswordUrl { get; init; }
}