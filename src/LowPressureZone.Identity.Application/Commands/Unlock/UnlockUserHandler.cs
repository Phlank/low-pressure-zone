using FastEndpoints;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Identity.Application.Commands.Unlock;

public class UnlockUserHandler(UserManager<AppUser> userManager) : ICommandHandler<UnlockUserCommand>
{
    public async Task ExecuteAsync(UnlockUserCommand command, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null) return;
        await userManager.SetLockoutEnabledAsync(user, false);
    }
}