using System.Net;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Users.Logout;

public sealed class GetLogoutTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Authenticated_user_can_sign_out()
    {
        var user = await App.CreateUserAsync("user");

        var response = await App.CreateClientFor(user).GetAsync(
            "/api/users/logout", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}
