using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.IntegrationTests.Infrastructure;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.IntegrationTests.Users.TwoFactor;

public sealed class PostTwoFactorTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Valid_email_code_completes_two_factor_sign_in()
    {
        var user = await UserTestData.CreateUserWithPasswordAsync(App, "two-factor-user", twoFactorEnabled: true);
        var client = App.CreateAnonymousClient();
        var login = await client.PostAsJsonAsync(
            "/api/users/login",
            new { username = user.UserName, password = UserTestData.ValidPassword },
            TestContext.Current.CancellationToken);
        login.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var scope = App.Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var savedUser = await userManager.FindByIdAsync(user.Id.ToString());
        savedUser.ShouldNotBeNull();
        var code = await userManager.GenerateTwoFactorTokenAsync(savedUser, "Email");

        var response = await client.PostAsJsonAsync(
            "/api/users/twofactor", new { code, rememberClient = false }, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Invalid_code_is_unauthorized()
    {
        var response = await App.CreateAnonymousClient().PostAsJsonAsync(
            "/api/users/twofactor", new { code = "invalid", rememberClient = false },
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
