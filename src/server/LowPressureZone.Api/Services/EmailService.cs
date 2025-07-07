using System.Text.Json;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using LowPressureZone.Api.Models.Options;
using LowPressureZone.Identity;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Api.Services;

public class EmailService(IOptions<EmailServiceOptions> options, UriService uriService, ISender sender, ILogger<EmailService> logger)
{
    private static readonly Action<ILogger, string, Exception?> LogEmailFailure = LoggerMessage.Define<string>(LogLevel.Error, new EventId(0, nameof(LogEmailFailure)), "Failed to send email: {Response}");
    private async Task Send(string toAddress, string subject, string body)
    {
        var email = Email.From(options.Value.FromAddress)
                         .To(toAddress)
                         .Subject(subject)
                         .Body(body);
        var sendResponse = await sender.SendAsync(email);
        if (!sendResponse.Successful) LogEmailFailure(logger, JsonSerializer.Serialize(sendResponse), null);
    }


    public async Task SendTwoFactorEmailAsync(string toAddress, string username, string code)
    {
        var subject = $"2FA | Low Pressure Zone";
        var message = $"Hey {username}, there was a login request made at Low Pressure Zone using your username and password.\n\nYour two factor authentication code is {code}.\n\nIf you weren't the one making the request, take the time to go to the site and change your password. Hope your day is going alright.";
        await Send(toAddress, subject, message);
    }

    public async Task SendInviteEmailAsync(string toAddress, TokenContext context)
    {
        var uri = uriService.GetInviteUrl(context);
        var subject = "Welcome | Low Pressure Zone";
        var message = $"You've been invited to register a new user at Low Pressure Zone. Follow the link below to create your user.\n\n{uri}\n\nThis link will be valid for 24 hours.";
        await Send(toAddress, subject, message);
    }

    public async Task SendResetPasswordEmailAsync(string toAddress, string username, TokenContext context)
    {
        var uri = uriService.GetResetPasswordUrl(context);
        var subject = "Reset Password | Low Pressure Zone";
        var message = $"A password reset was requested from Low Pressure Zone for your user, {username}. Click on the link below to reset your password.\n\n{uri}\n\nIf you did not initiate this request, you can ignore this message.";
        await Send(toAddress, subject, message);
    }
}
