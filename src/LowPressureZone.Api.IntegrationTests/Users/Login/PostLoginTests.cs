using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.Users.Login;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Users.Login;

public sealed class PostLoginTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task User_can_sign_in_with_valid_credentials()
    {
        await UserTestData.CreateUserWithPasswordAsync(App, "login-user");

        var response = await App.CreateAnonymousClient().PostAsJsonAsync(
            "/api/users/login",
            new { username = "login-user", password = UserTestData.ValidPassword },
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken);
        body.ShouldNotBeNull();
        body.RequiresTwoFactor.ShouldBeFalse();
    }

    [Fact]
    public async Task Invalid_credentials_are_forbidden()
    {
        await UserTestData.CreateUserWithPasswordAsync(App, "login-user");

        var response = await App.CreateAnonymousClient().PostAsJsonAsync(
            "/api/users/login",
            new { username = "login-user", password = "wrong-password" },
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Two_factor_user_is_prompted_for_second_factor()
    {
        await UserTestData.CreateUserWithPasswordAsync(App, "two-factor-user", twoFactorEnabled: true);

        var response = await App.CreateAnonymousClient().PostAsJsonAsync(
            "/api/users/login",
            new { username = "two-factor-user", password = UserTestData.ValidPassword },
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken);
        body.ShouldNotBeNull();
        body.RequiresTwoFactor.ShouldBeTrue();
    }
}
