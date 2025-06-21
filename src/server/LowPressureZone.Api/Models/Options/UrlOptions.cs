namespace LowPressureZone.Api.Models.Options;

public class UrlOptions
{
    public const string Name = "Urls";

    public required Uri RegisterUrl { get; set; }
    public required Uri ResetPasswordUrl { get; set; }
}
