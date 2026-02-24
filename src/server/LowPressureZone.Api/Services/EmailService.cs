using System.Text.Json;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using LowPressureZone.Api.Models.Configuration;
using LowPressureZone.Core;
using LowPressureZone.Identity;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Api.Services;

public sealed class EmailService(
    IOptions<EmailServiceConfiguration> options,
    UriService uriService,
    IFluentEmail email,
    ILogger<EmailService> logger)
{
    private static readonly Action<ILogger, string, Exception?> LogEmailFailure =
        LoggerMessage.Define<string>(LogLevel.Error, new EventId(0, nameof(LogEmailFailure)),
                                     "Failed to send email: {Response}");

    private async Task<Result<SendResponse, SendResponse>> SendAsync(string toAddress, string subject, string body)
    {
        var sendResponse = await email.SetFrom(options.Value.FromAddress)
                                      .To(toAddress)
                                      .Subject(subject)
                                      .Body(body)
                                      .SendAsync();
        if (sendResponse.Successful)
            return Result.Ok<SendResponse, SendResponse>(sendResponse);

        LogEmailFailure(logger, JsonSerializer.Serialize(sendResponse), null);
        // Don't send an admin message regarding email failure if the failed email was an admin message
        var adminEmailBody = "Begin errors";
        adminEmailBody += "\n" + string.Join("\n", sendResponse.ErrorMessages);
        if (!subject.Contains("Admin Message", StringComparison.InvariantCulture))
            _ = await SendAdminMessage(adminEmailBody, "Error sending email");
        return Result.Err<SendResponse, SendResponse>(sendResponse);
    }


    public async Task SendTwoFactorEmailAsync(string toAddress, string username, string code)
    {
        var subject = "2FA | Low Pressure Zone";
        var message =
            $"Hey {username}, there was a login request made at Low Pressure Zone using your username and password.\n\nYour two factor authentication code is {code}.\n\nIf you weren't the one making the request, take the time to go to the site and change your password. Hope your day is going alright.";
        _ = await SendAsync(toAddress, subject, message);
    }

    public async Task SendInviteEmailAsync(string toAddress, TokenContext context)
    {
        var uri = uriService.GetInviteUrl(context);
        var subject = "Welcome | Low Pressure Zone";
        var message =
            $"You've been invited to register a new user at Low Pressure Zone. Follow the link below to create your user.\n\n{uri}\n\nThis link will be valid for 24 hours.";
        _ = await SendAsync(toAddress, subject, message);
    }

    public async Task SendResetPasswordEmailAsync(string toAddress, string username, TokenContext context)
    {
        var uri = uriService.GetResetPasswordUrl(context);
        var subject = "Reset Password | Low Pressure Zone";
        var message =
            $"A password reset was requested from Low Pressure Zone for your user, {username}. Click on the link below to reset your password.\n\n{uri}\n\nIf you did not initiate this request, you can ignore this message.";
        _ = await SendAsync(toAddress, subject, message);
    }

    public async Task<Result<bool, SendResponse>> SendAdminMessage(string message, string subject)
    {
        var adminEmail = options.Value.AdminEmail;
        subject += " | Admin Message | Low Pressure Zone";
        if (!subject.EndsWith("Low Pressure Zone", StringComparison.InvariantCulture))
            subject += " - Low Pressure Zone";
        var result = await SendAsync(adminEmail, subject, message);
        if (!result.IsSuccess) return Result.Err<bool, SendResponse>(result.Error);
        return Result.Ok<bool, SendResponse>(true);
    }
}