using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Services;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.ResetPassword;

public class GetPasswordReset(UserManager<AppUser> userManager, EmailService emailService) : Endpoint<GetResetPasswordRequest, GetResetPasswordResponse>
{
    private readonly DateTime _start = DateTime.UtcNow;

    public override void Configure()
    {
        Get("/users/resetpassword");
        Throttle(3, 60);
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetResetPasswordRequest req, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(req.Email);
        if (user == null)
        {
            await this.SendDelayedNoContentAsync(_start, ct);
            return;
        }

        if (!await userManager.HasPasswordAsync(user))
        {
            await this.SendDelayedNoContentAsync(_start, ct);
            return;
        }

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var context = new TokenContext
        {
            Email = user.Email!,
            Token = resetToken
        };
        await emailService.SendResetPasswordEmail(user.Email!, user.UserName!, context);
    }
}
