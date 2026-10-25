using System.Net;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Api.IntegrationTests.Infrastructure;
using static LowPressureZone.Api.IntegrationTests.Infrastructure.TestData;

namespace LowPressureZone.Api.IntegrationTests.Broadcasts;

public sealed class DownloadBroadcastTests(ApiFixture app) : ApiTestBase(app)
{
    private const int StreamerId = 10;
    private const int BroadcastId = 3;
    private static readonly DateTime Start = new(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
    private static readonly byte[] RecordingBytes = [1, 2, 3, 4, 5];

    private static string DownloadUrl(int broadcastId = BroadcastId, int streamerId = StreamerId)
        => $"/api/broadcasts/download?broadcastId={broadcastId}&streamerId={streamerId}";

    private void SetUpAzuraCastWithRecording()
    {
        A.CallTo(() => AzuraCast.GetStreamerAsync(StreamerId))
         .Returns(Ok(new StationStreamer { Id = StreamerId, StreamerUsername = "dj_one", DisplayName = "DJ One" }));
        A.CallTo(() => AzuraCast.GetBroadcastsAsync(StreamerId))
         .Returns(BroadcastsResult(RemoteBroadcast(BroadcastId, StreamerId, "DJ One", Start, Start.AddHours(1))));
        A.CallTo(() => AzuraCast.DownloadBroadcastFileAsync(StreamerId, BroadcastId))
         .ReturnsLazily(() => Ok<HttpContent>(new ByteArrayContent(RecordingBytes)));
    }

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var response = await App.CreateAnonymousClient()
                                .GetAsync(DownloadUrl(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Performer_cannot_download_another_streamers_broadcast()
    {
        var user = await App.CreateUserAsync("performer", streamerId: 20);
        SetUpAzuraCastWithRecording();

        var response = await App.CreateClientFor(user).GetAsync(DownloadUrl(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        A.CallTo(() => AzuraCast.DownloadBroadcastFileAsync(A<int>._, A<int>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Performer_downloads_own_broadcast_recording()
    {
        var user = await App.CreateUserAsync("performer", streamerId: StreamerId);
        SetUpAzuraCastWithRecording();

        var response = await App.CreateClientFor(user).GetAsync(DownloadUrl(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var contentDisposition = response.Content.Headers.ContentDisposition.ShouldNotBeNull();
        (contentDisposition.FileNameStar ?? contentDisposition.FileName?.Trim('"'))
            .ShouldBe("DJ One 2026-01-02_03-04-05.mp3");
        (await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken)).ShouldBe(RecordingBytes);
    }

    [Fact]
    public async Task Admin_downloads_any_broadcast_recording()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        SetUpAzuraCastWithRecording();

        var response = await App.CreateClientFor(admin).GetAsync(DownloadUrl(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken)).ShouldBe(RecordingBytes);
    }

    [Fact]
    public async Task Organizer_downloads_any_broadcast_recording()
    {
        var organizer = await App.CreateUserAsync("organizer");
        await App.MakeOrganizerAsync(organizer);
        SetUpAzuraCastWithRecording();

        var response = await App.CreateClientFor(organizer)
                                .GetAsync(DownloadUrl(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Unknown_streamer_is_a_bad_request()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        A.CallTo(() => AzuraCast.GetStreamerAsync(StreamerId))
         .Returns(Error<StationStreamer>(HttpStatusCode.NotFound));
        A.CallTo(() => AzuraCast.GetBroadcastsAsync(StreamerId))
         .Returns(Error<IReadOnlyCollection<StationStreamerBroadcast>>(HttpStatusCode.NotFound));

        var response = await App.CreateClientFor(admin).GetAsync(DownloadUrl(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        body.ShouldContain("streamerId");
        body.ShouldContain("Streamer not found");
    }

    [Fact]
    public async Task Unknown_broadcast_is_not_found()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        SetUpAzuraCastWithRecording();

        var response = await App.CreateClientFor(admin)
                                .GetAsync(DownloadUrl(broadcastId: 999), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        A.CallTo(() => AzuraCast.DownloadBroadcastFileAsync(A<int>._, A<int>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Broadcast_without_recording_is_not_found()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        SetUpAzuraCastWithRecording();
        A.CallTo(() => AzuraCast.GetBroadcastsAsync(StreamerId))
         .Returns(BroadcastsResult(RemoteBroadcast(BroadcastId, StreamerId, "DJ One", Start, hasRecording: false)));

        var response = await App.CreateClientFor(admin).GetAsync(DownloadUrl(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        A.CallTo(() => AzuraCast.DownloadBroadcastFileAsync(A<int>._, A<int>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Failed_AzuraCast_download_is_not_found()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        SetUpAzuraCastWithRecording();
        A.CallTo(() => AzuraCast.DownloadBroadcastFileAsync(StreamerId, BroadcastId))
         .Returns(Error<HttpContent>(HttpStatusCode.InternalServerError));

        var response = await App.CreateClientFor(admin).GetAsync(DownloadUrl(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
