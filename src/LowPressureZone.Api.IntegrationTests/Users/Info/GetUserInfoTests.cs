using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.Users.Info;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Users.Info;

public sealed class GetUserInfoTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var response = await App.CreateAnonymousClient().GetAsync("/api/users/info", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Authenticated_user_receives_claims_and_streamer_id()
    {
        var user = await App.CreateUserAsync("performer", streamerId: 45);

        var response = await App.CreateClientFor(user).GetAsync("/api/users/info", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var info = await response.Content.ReadFromJsonAsync<UserInfoResponse>(TestContext.Current.CancellationToken);
        info.ShouldNotBeNull();
        info.Id.ShouldBe(user.Id.ToString());
        info.Username.ShouldBe(user.UserName);
        info.Email.ShouldBe(user.Email);
        info.StreamerId.ShouldBe(45);
        info.Roles.ShouldBeEmpty();
    }
}
