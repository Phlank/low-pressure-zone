using System.Text.Json;
using FluentEmail.Core;
using FluentEmail.Mailgun;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Api.Services;

public class EmailService
{
    private readonly string _fromAddress;
    private readonly MailgunSender _sender;

    public EmailService(EmailServiceConfiguration configuration, MailgunSender sender)
    {
        _fromAddress = configuration.FromAddress;
        _sender = sender;
    }

    private async Task Send(string toAddress, string subject, string body)
    {
        var email = Email.From(_fromAddress)
                         .To(toAddress)
                         .Subject(subject)
                         .Body(body);
        var response = await _sender.SendAsync(email);
    }

    public async Task SendTwoFactorEmail(string toAddress, string username, string code)
    {
        var subject = "2FA | Low Pressure Zone";
        var message = $"Hey {username}, there was a login request made at Low Pressure Zone using your username and password.\n\nYour two factor authentication code is {code}.\n\nIf you weren't the one making the request, take the time to go to the site and change your password. Hope your day is going alright.";
        await Send(toAddress, subject, message);
    }

    public async Task SendInviteEmail(string toAddress, string registerUrl)
    {
        var subject = "Welcome | Low Pressure Zone";
        var message = $"You've been invited to register a new user at Low Pressure Zone. Follow the link below to create your user.\n\n{registerUrl}\n\nThis link will be valid for 24 hours.";
        await Send(toAddress, subject, message);
    }
}
