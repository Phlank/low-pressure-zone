using System.Net;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Api.Endpoints.Stream.ConnectionInfo;
using LowPressureZone.Api.IntegrationTests.Infrastructure;
using static LowPressureZone.Api.IntegrationTests.Infrastructure.TestData;

namespace LowPressureZone.Api.IntegrationTests.Stream.ConnectionInfo;

public sealed class GetConnectionInfoTests(ApiFixture app) : ApiTestBase(app)
{
    private const int StreamerId = 17;
    private const string Route = "/api/stream/connectioninfo";

    [Fact]
    public async Task Linked_user_receives_live_and_test_connection_details()
    {
        var user = await App.CreateUserAsync("performer", streamerId: StreamerId);
        A.CallTo(() => AzuraCast.GetStreamerAsync(StreamerId))
         .Returns(Ok(new StationStreamer
         {
             Id = StreamerId,
             StreamerUsername = "dj_test",
             DisplayName = "DJ Test"
         }));

        var response = await App.CreateClientFor(user).GetAsync(Route, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<ConnectionInfoResponse>>(
            TestContext.Current.CancellationToken);
        body.ShouldNotBeNull();
        body.Count.ShouldBe(2);

        var live = body[0];
        live.StreamType.ShouldBe("Live Stream");
        live.Host.ShouldBe("live.example.test");
        live.Port.ShouldBe("8000");
        live.Mount.ShouldBe("/radio.mp3");
        live.Username.ShouldBe("dj_test");
        live.Password.ShouldBeNull();
        live.Type.ShouldBe("AzuraCast");
        live.DisplayName.ShouldBe("DJ Test");
        live.IsDisplayNameEditable.ShouldBeTrue();

        var test = body[1];
        test.StreamType.ShouldBe("Test Stream");
        test.Host.ShouldBe("test.example.test");
        test.Port.ShouldBe("8001");
        test.Mount.ShouldBe("/test.mp3");
        test.Username.ShouldBe("test-user");
        test.Password.ShouldBe("test-password");
        test.Type.ShouldBe("Icecast");
        test.DisplayName.ShouldBeNull();
        test.IsDisplayNameEditable.ShouldBeFalse();

        A.CallTo(() => AzuraCast.GetStreamerAsync(StreamerId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Missing_display_name_falls_back_to_streamer_username()
    {
        var user = await App.CreateUserAsync("performer", streamerId: StreamerId);
        A.CallTo(() => AzuraCast.GetStreamerAsync(StreamerId))
         .Returns(Ok(new StationStreamer { Id = StreamerId, StreamerUsername = "dj_test" }));

        var response = await App.CreateClientFor(user).GetAsync(Route, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<List<ConnectionInfoResponse>>(
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        body.ShouldNotBeNull();
        body[0].DisplayName.ShouldBe("dj_test");
    }

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var response = await App.CreateAnonymousClient().GetAsync(Route, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        A.CallTo(() => AzuraCast.GetStreamerAsync(A<int>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task User_without_linked_streamer_gets_an_error()
    {
        var user = await App.CreateUserAsync("performer");

        var response = await App.CreateClientFor(user).GetAsync(Route, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))
            .ShouldContain("User is not linked to streamer");
        A.CallTo(() => AzuraCast.GetStreamerAsync(A<int>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task AzuraCast_failure_is_returned_as_bad_request()
    {
        var user = await App.CreateUserAsync("performer", streamerId: StreamerId);
        A.CallTo(() => AzuraCast.GetStreamerAsync(StreamerId))
         .Returns(Error<StationStreamer>(HttpStatusCode.ServiceUnavailable));

        var response = await App.CreateClientFor(user).GetAsync(Route, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))
            .ShouldContain("Unable to get streamer");
    }
}
