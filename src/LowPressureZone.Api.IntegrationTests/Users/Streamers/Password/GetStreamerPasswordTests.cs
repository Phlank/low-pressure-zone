using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Api.Endpoints.Users.Streamers.Password;
using LowPressureZone.Api.IntegrationTests.Infrastructure;
using static LowPressureZone.Api.IntegrationTests.Infrastructure.TestData;

namespace LowPressureZone.Api.IntegrationTests.Users.Streamers.Password;

public sealed class GetStreamerPasswordTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Linked_user_gets_new_streamer_password()
    {
        const int streamerId = 31;
        var user = await App.CreateUserAsync("performer", streamerId);
        A.CallTo(() => AzuraCast.GetStreamerAsync(streamerId))
         .Returns(Ok(new StationStreamer { Id = streamerId, StreamerUsername = "performer" }));
        A.CallTo(() => AzuraCast.PutStreamerAsync(A<StationStreamer>._)).Returns(Ok(true));

        var response = await App.CreateClientFor(user).GetAsync(
            "/api/users/streamers/password", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<StreamerPasswordResponse>(
            TestContext.Current.CancellationToken);
        body.ShouldNotBeNull();
        body.Password.ShouldNotBeNullOrEmpty();
        body.Password.Length.ShouldBe(15);
        A.CallTo(() => AzuraCast.PutStreamerAsync(A<StationStreamer>.That.Matches(
                   streamer => streamer.StreamerPassword == body.Password)))
         .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var response = await App.CreateAnonymousClient().GetAsync(
            "/api/users/streamers/password", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
