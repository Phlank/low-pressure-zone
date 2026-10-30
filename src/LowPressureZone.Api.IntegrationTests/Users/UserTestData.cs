global using Microsoft.Extensions.DependencyInjection;

using LowPressureZone.Api.IntegrationTests.Infrastructure;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Entities;
using LowPressureZone.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.IntegrationTests.Users;

internal static class UserTestData
{
    public const string ValidPassword = "SafePassw0rd!";

    public static async Task<AppUser> CreateUserWithPasswordAsync(
        ApiFixture app,
        string username,
        bool twoFactorEnabled = false)
    {
        var user = await app.CreateUserAsync(username);
        await using var scope = app.Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        user = await userManager.FindByIdAsync(user.Id.ToString()) ?? throw new InvalidOperationException();
        (await userManager.AddPasswordAsync(user, ValidPassword)).Succeeded.ShouldBeTrue();

        if (twoFactorEnabled)
        {
            user.TwoFactorEnabled = true;
            (await userManager.UpdateAsync(user)).Succeeded.ShouldBeTrue();
        }

        return user;
    }

    public static async Task<AppUser> CreateInvitedUserAsync(ApiFixture app, string email)
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = Guid.NewGuid().ToString(),
            Email = email,
            EmailConfirmed = false,
            DisplayName = email,
            LockoutEnabled = true,
            LockoutEnd = DateTimeOffset.MaxValue
        };

        await using var scope = app.Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        (await userManager.CreateAsync(user)).Succeeded.ShouldBeTrue();

        var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        identityContext.Invitations.Add(new Invitation<Guid, AppUser>
        {
            UserId = user.Id,
            InvitationDate = DateTime.UtcNow
        });
        await identityContext.SaveChangesAsync();
        return user;
    }

    public static async Task<string> CreateResetContextAsync(ApiFixture app, Guid userId)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var user = await userManager.FindByIdAsync(userId.ToString());
        user.ShouldNotBeNull();
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        return new TokenContext { Email = user.Email!, Token = token }.Encoded;
    }

    public static async Task<string> CreateInviteContextAsync(ApiFixture app, Guid userId)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var user = await userManager.FindByIdAsync(userId.ToString());
        user.ShouldNotBeNull();
        var token = await userManager.GenerateUserTokenAsync(user, "Default", "Invite");
        return new TokenContext { Email = user.Email!, Token = token }.Encoded;
    }
}
