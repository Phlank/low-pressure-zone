namespace LowPressureZone.Api.Services;

public class EmailServiceOptions
{
    public const string Name = "Email";

    public required string MailgunApiKey { get; set; }
    public required string MailgunDomain { get; set; }
    public required string FromAddress { get; set; }
}
