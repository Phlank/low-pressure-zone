using FluentEmail.Core.Models;
using LowPressureZone.Core;
using LowPressureZone.Identity;

namespace LowPressureZone.Api.Services;

public interface IEmailService
{
    Task SendTwoFactorEmailAsync(string toAddress, string username, string code);
    Task SendInviteEmailAsync(string toAddress, TokenContext context);
    Task SendResetPasswordEmailAsync(string toAddress, string username, TokenContext context);
    Task<Result<bool, SendResponse>> SendAdminMessage(string message, string subject);
    Task<Result<NoValue, SendResponse>> SendAdminException(Exception ex, string process);
}