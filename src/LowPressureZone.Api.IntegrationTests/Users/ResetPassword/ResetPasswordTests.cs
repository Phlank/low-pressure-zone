using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.IntegrationTests.Infrastructure;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.IntegrationTests.Users.ResetPassword;

public sealed class ResetPasswordTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Password_reset_changes_the_password()
    {
        var user = await UserTestData.CreateUserWithPasswordAsync(App, "reset-user");
        var context = await UserTestData.CreateResetContextAsync(App, user.Id);
        const string newPassword = "NewPassw0rd!";

        var response = await App.CreateAnonymousClient().PostAsJsonAsync(
            "/api/users/resetpassword", new { context, password = newPassword }, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        await using var scope = App.Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var updatedUser = await userManager.FindByIdAsync(user.Id.ToString());
        updatedUser.ShouldNotBeNull();
        (await userManager.CheckPasswordAsync(updatedUser, newPassword)).ShouldBeTrue();
        (await userManager.CheckPasswordAsync(updatedUser, UserTestData.ValidPassword)).ShouldBeFalse();
    }

    [Fact]
    public async Task Password_that_fails_validation_does_not_replace_the_existing_password()
    {
        var user = await UserTestData.CreateUserWithPasswordAsync(App, "reset-user");
        var context = await UserTestData.CreateResetContextAsync(App, user.Id);

        var response = await App.CreateAnonymousClient().PostAsJsonAsync(
            "/api/users/resetpassword", new { context, password = "weak" }, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await using var scope = App.Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var unchangedUser = await userManager.FindByIdAsync(user.Id.ToString());
        unchangedUser.ShouldNotBeNull();
        (await userManager.CheckPasswordAsync(unchangedUser, UserTestData.ValidPassword)).ShouldBeTrue();
    }

    [Fact]
    public async Task Malformed_reset_context_is_forbidden()
    {
        var response = await App.CreateAnonymousClient().PostAsJsonAsync(
            "/api/users/resetpassword", new { context = "invalid", password = "NewPassw0rd!" },
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Reset_request_returns_empty_response_for_unknown_email()
    {
        var response = await App.CreateAnonymousClient().GetAsync(
            "/api/users/resetpassword?email=unknown%40example.com", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Reset_request_sends_email_without_returning_a_temporary_password()
    {
        await UserTestData.CreateUserWithPasswordAsync(App, "reset-user");

        var response = await App.CreateAnonymousClient().GetAsync(
            "/api/users/resetpassword?email=reset-user%40example.com", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        body.ShouldBeEmpty();
    }
}
