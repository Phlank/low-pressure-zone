using System.Net;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Api.IntegrationTests.Infrastructure;
using LowPressureZone.Identity;
using Microsoft.EntityFrameworkCore;
using static LowPressureZone.Api.IntegrationTests.Infrastructure.TestData;

namespace LowPressureZone.Api.IntegrationTests.Users.Streamers.Link;

public sealed class PostLinkStreamersTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Admin_links_all_users_and_ignores_partial_failures()
    {
        var admin = await App.CreateUserAsync("admin", streamerId: 900, isAdmin: true);
        var successfulUser = await App.CreateUserAsync("successful");
        var failedUser = await App.CreateUserAsync("failed");
        A.CallTo(() => AzuraCast.PostStreamerAsync(successfulUser.UserName!, A<string>._, successfulUser.DisplayName))
         .Returns(Ok(72));
        A.CallTo(() => AzuraCast.PostStreamerAsync(failedUser.UserName!, A<string>._, failedUser.DisplayName))
         .Returns(Error<int>(HttpStatusCode.ServiceUnavailable));
        A.CallTo(() => AzuraCast.GetStreamersAsync())
         .Returns(Ok<IReadOnlyCollection<StationStreamer>>([]));

        var response = await App.CreateClientFor(admin).PostAsync(
            "/api/users/streamers/link", null, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        await using var scope = App.Services.CreateAsyncScope();
        var users = scope.ServiceProvider.GetRequiredService<IdentityContext>().Users.AsNoTracking().ToList();
        users.Single(user => user.Id == successfulUser.Id).StreamerId.ShouldBe(72);
        users.Single(user => user.Id == failedUser.Id).StreamerId.ShouldBeNull();
    }

    [Fact]
    public async Task Admin_gets_error_when_every_link_fails()
    {
        var admin = await App.CreateUserAsync("admin", streamerId: 900, isAdmin: true);
        var target = await App.CreateUserAsync("target");
        A.CallTo(() => AzuraCast.PostStreamerAsync(target.UserName!, A<string>._, target.DisplayName))
         .Returns(Error<int>(HttpStatusCode.ServiceUnavailable));
        A.CallTo(() => AzuraCast.GetStreamersAsync())
         .Returns(Ok<IReadOnlyCollection<StationStreamer>>([]));

        var response = await App.CreateClientFor(admin).PostAsync(
            "/api/users/streamers/link", null, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))
            .ShouldContain("Failed to create or link streamer for 1 of  1 users");
    }

    [Fact]
    public async Task Non_admin_cannot_link_all_streamers()
    {
        var user = await App.CreateUserAsync("user");

        var response = await App.CreateClientFor(user).PostAsync(
            "/api/users/streamers/link", null, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
