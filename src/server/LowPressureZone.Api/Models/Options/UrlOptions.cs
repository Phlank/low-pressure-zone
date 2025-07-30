namespace LowPressureZone.Api.Models.Options;

public class UrlOptions
{
    public const string Name = "Urls";

    public required Uri RegisterUrl { get; init; }
    public required Uri ResetPasswordUrl { get; init; }
}
