using System.Net;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Api.IntegrationTests.Infrastructure;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using static LowPressureZone.Api.IntegrationTests.Infrastructure.TestData;

namespace LowPressureZone.Api.IntegrationTests.Users.Streamers.Link;

public sealed class PostLinkStreamerTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Admin_links_user_to_new_streamer()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var target = await App.CreateUserAsync("target");
        A.CallTo(() => AzuraCast.PostStreamerAsync(target.UserName!, A<string>._, target.DisplayName))
         .Returns(Ok(42));

        var response = await App.CreateClientFor(admin).PostAsync(
            $"/api/users/streamers/link/{target.Id}", null, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        await using var scope = App.Services.CreateAsyncScope();
        var linkedUser = await scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>()
                               .FindByIdAsync(target.Id.ToString());
        linkedUser.ShouldNotBeNull();
        linkedUser.StreamerId.ShouldBe(42);
    }

    [Fact]
    public async Task Existing_matching_streamer_is_used_when_creation_fails()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var target = await App.CreateUserAsync("target");
        A.CallTo(() => AzuraCast.PostStreamerAsync(target.UserName!, A<string>._, target.DisplayName))
         .Returns(Error<int>(HttpStatusCode.Conflict));
        A.CallTo(() => AzuraCast.GetStreamersAsync())
         .Returns(Ok<IReadOnlyCollection<StationStreamer>>(
             [new StationStreamer { Id = 51, StreamerUsername = target.UserName! }]));

        var response = await App.CreateClientFor(admin).PostAsync(
            $"/api/users/streamers/link/{target.Id}", null, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        await using var scope = App.Services.CreateAsyncScope();
        var linkedUser = await scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>()
                               .FindByIdAsync(target.Id.ToString());
        linkedUser.ShouldNotBeNull();
        linkedUser.StreamerId.ShouldBe(51);
    }

    [Fact]
    public async Task Non_admin_cannot_link_streamers()
    {
        var user = await App.CreateUserAsync("user");

        var response = await App.CreateClientFor(user).PostAsync(
            $"/api/users/streamers/link/{Guid.NewGuid()}", null, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Missing_user_is_not_found()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);

        var response = await App.CreateClientFor(admin).PostAsync(
            $"/api/users/streamers/link/{Guid.NewGuid()}", null, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
