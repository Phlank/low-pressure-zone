namespace LowPressureZone.Api.Services;

public class UriServiceOptions
{
    public const string Name = "Urls";

    public required Uri RegisterUrl { get; set; }
    public required Uri ResetPasswordUrl { get; set; }
}