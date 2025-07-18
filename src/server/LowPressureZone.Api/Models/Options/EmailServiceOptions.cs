﻿namespace LowPressureZone.Api.Models.Options;

public class EmailServiceOptions
{
    public const string Name = "Email";

    public required string MailgunApiKey { get; set; }
    public required string MailgunDomain { get; set; }
    public required string FromAddress { get; set; }
    public required string AdminEmail { get; set; }
}
