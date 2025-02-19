namespace LowPressureZone.Api.Services;

public class EmailServiceConfiguration
{
    public required string MailgunApiKey { get; set; }
    public required string MailgunDomain { get; set; }
    public required string FromAddress { get; set; }
    public required string RegisterUrl { get; set; }
}
