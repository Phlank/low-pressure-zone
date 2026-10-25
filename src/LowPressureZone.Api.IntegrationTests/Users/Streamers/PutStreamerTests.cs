using System.Net;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Api.IntegrationTests.Infrastructure;
using static LowPressureZone.Api.IntegrationTests.Infrastructure.TestData;

namespace LowPressureZone.Api.IntegrationTests.Users.Streamers;

public sealed class PutStreamerTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Linked_user_updates_streamer_display_name()
    {
        const int streamerId = 23;
        var user = await App.CreateUserAsync("performer", streamerId);
        A.CallTo(() => AzuraCast.GetStreamerAsync(streamerId))
         .Returns(Ok(new StationStreamer { Id = streamerId, StreamerUsername = "performer" }));
        A.CallTo(() => AzuraCast.PutStreamerAsync(A<StationStreamer>._)).Returns(Ok(true));

        var response = await App.CreateClientFor(user).PutAsJsonAsync(
            "/api/users/streamers", new { displayName = "DJ Performer" }, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        A.CallTo(() => AzuraCast.PutStreamerAsync(A<StationStreamer>.That.Matches(
                   streamer => streamer.DisplayName == "DJ Performer" && streamer.Id == streamerId)))
         .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task User_without_linked_streamer_is_unauthorized()
    {
        var user = await App.CreateUserAsync("performer");

        var response = await App.CreateClientFor(user).PutAsJsonAsync(
            "/api/users/streamers", new { displayName = "DJ Performer" }, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        A.CallTo(() => AzuraCast.PutStreamerAsync(A<StationStreamer>._)).MustNotHaveHappened();
    }
}
