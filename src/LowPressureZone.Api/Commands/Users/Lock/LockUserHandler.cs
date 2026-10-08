using FastEndpoints;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Commands.Users.Lock;

public class LockUserHandler(UserManager<AppUser> userManager) : ICommandHandler<LockUserCommand>
{
    public async Task ExecuteAsync(LockUserCommand command, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null) return;
        await userManager.SetLockoutEnabledAsync(user, true);
    }
}