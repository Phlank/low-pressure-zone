using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.Users;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Users;

public sealed class GetUsersTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var response = await App.CreateAnonymousClient().GetAsync("/api/users", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Non_admin_sees_non_admin_users_only()
    {
        var caller = await App.CreateUserAsync("caller");
        var member = await App.CreateUserAsync("member", streamerId: 12);
        var admin = await App.CreateUserAsync("admin", isAdmin: true);

        var response = await App.CreateClientFor(caller).GetAsync("/api/users", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>(TestContext.Current.CancellationToken);
        users.ShouldNotBeNull();
        users.Select(user => user.Id).ShouldContain(caller.Id);
        users.Select(user => user.Id).ShouldContain(member.Id);
        users.Select(user => user.Id).ShouldNotContain(admin.Id);
        users.Single(user => user.Id == member.Id).IsStreamer.ShouldBeTrue();
    }

    [Fact]
    public async Task Admin_sees_admin_users()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var otherAdmin = await App.CreateUserAsync("other-admin", isAdmin: true);

        var response = await App.CreateClientFor(admin).GetAsync("/api/users", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>(TestContext.Current.CancellationToken);
        users.ShouldNotBeNull();
        users.Single(user => user.Id == otherAdmin.Id).IsAdmin.ShouldBeTrue();
    }
}
