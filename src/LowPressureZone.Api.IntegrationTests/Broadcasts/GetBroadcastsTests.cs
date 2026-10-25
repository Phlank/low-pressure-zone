using System.Net;
using LowPressureZone.Api.Endpoints.Broadcasts;
using LowPressureZone.Api.IntegrationTests.Infrastructure;
using static LowPressureZone.Api.IntegrationTests.Infrastructure.TestData;

namespace LowPressureZone.Api.IntegrationTests.Broadcasts;

public sealed class GetBroadcastsTests(ApiFixture app) : ApiTestBase(app)
{
    private static readonly DateTime Day = new(2026, 3, 14, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var (response, _) = await App.CreateAnonymousClient()
                                     .GETAsync<GetBroadcasts, IEnumerable<BroadcastResponse>>();

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task User_without_streamer_is_forbidden()
    {
        var user = await App.CreateUserAsync("no_streamer");

        var (response, _) = await App.CreateClientFor(user)
                                     .GETAsync<GetBroadcasts, IEnumerable<BroadcastResponse>>();

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        A.CallTo(() => AzuraCast.GetBroadcastsAsync(A<int?>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Performer_only_receives_own_broadcasts_newest_first()
    {
        var user = await App.CreateUserAsync("performer", streamerId: 10);
        A.CallTo(() => AzuraCast.GetBroadcastsAsync(A<int?>._))
         .Returns(BroadcastsResult(RemoteBroadcast(1, 10, "Performer", Day.AddHours(1), Day.AddHours(2)),
                                   RemoteBroadcast(2, 20, "Someone Else", Day.AddHours(3), Day.AddHours(4)),
                                   RemoteBroadcast(3, 10, "Performer", Day.AddHours(5), hasRecording: false)));

        var (response, result) = await App.CreateClientFor(user)
                                          .GETAsync<GetBroadcasts, List<BroadcastResponse>>();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Select(b => b.BroadcastId).ShouldBe([3, 1]);
        result.ShouldAllBe(b => b.StreamerId == 10 && !b.IsArchivable && !b.IsDisconnectable);

        var live = result[0];
        live.IsDownloadable.ShouldBeFalse();
        live.End.ShouldBeNull();

        var finished = result[1];
        finished.IsDownloadable.ShouldBeTrue();
        finished.StreamerDisplayName.ShouldBe("Performer");
        finished.Start.ShouldBe(new DateTimeOffset(Day.AddHours(1)));
        finished.End.ShouldBe(new DateTimeOffset(Day.AddHours(2)));
    }

    [Fact]
    public async Task Admin_receives_all_broadcasts_with_management_permissions()
    {
        var admin = await App.CreateUserAsync("admin", streamerId: 99, isAdmin: true);
        await App.AddBroadcastsAsync(LocalBroadcast(2, 20, "Archived", Day.AddHours(3), Day.AddHours(4),
                                                    isArchived: true));
        A.CallTo(() => AzuraCast.GetBroadcastsAsync(A<int?>._))
         .Returns(BroadcastsResult(RemoteBroadcast(1, 10, "Recorded", Day.AddHours(1), Day.AddHours(2)),
                                   RemoteBroadcast(2, 20, "Archived", Day.AddHours(3), Day.AddHours(4)),
                                   RemoteBroadcast(3, 30, "Live", Day.AddHours(5), hasRecording: false)));

        var (response, result) = await App.CreateClientFor(admin)
                                          .GETAsync<GetBroadcasts, List<BroadcastResponse>>();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Select(b => b.BroadcastId).ShouldBe([3, 2, 1]);

        var live = result.Single(b => b.BroadcastId == 3);
        live.IsDisconnectable.ShouldBeTrue();
        live.IsArchivable.ShouldBeFalse();

        var archived = result.Single(b => b.BroadcastId == 2);
        archived.IsArchivable.ShouldBeFalse();
        archived.IsDisconnectable.ShouldBeFalse();

        var recorded = result.Single(b => b.BroadcastId == 1);
        recorded.IsArchivable.ShouldBeTrue();
        recorded.IsDownloadable.ShouldBeTrue();
        recorded.IsDisconnectable.ShouldBeFalse();
    }

    [Fact]
    public async Task Organizer_receives_all_broadcasts()
    {
        var organizer = await App.CreateUserAsync("organizer", streamerId: 10);
        await App.MakeOrganizerAsync(organizer);
        A.CallTo(() => AzuraCast.GetBroadcastsAsync(A<int?>._))
         .Returns(BroadcastsResult(RemoteBroadcast(1, 10, "Organizer", Day.AddHours(1), Day.AddHours(2)),
                                   RemoteBroadcast(2, 20, "Someone Else", Day.AddHours(3), Day.AddHours(4))));

        var (response, result) = await App.CreateClientFor(organizer)
                                          .GETAsync<GetBroadcasts, List<BroadcastResponse>>();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Select(b => b.BroadcastId).ShouldBe([2, 1]);
        result.ShouldAllBe(b => b.IsArchivable);
    }

    [Fact]
    public async Task Broadcasts_are_synchronized_with_AzuraCast()
    {
        var user = await App.CreateUserAsync("performer", streamerId: 10);
        await App.AddBroadcastsAsync(LocalBroadcast(1, 10, "Performer", Day.AddHours(1), Day.AddHours(2)),
                                     LocalBroadcast(5, 10, "Performer", Day.AddHours(-5), Day.AddHours(-4)));
        A.CallTo(() => AzuraCast.GetBroadcastsAsync(A<int?>._))
         .Returns(BroadcastsResult(RemoteBroadcast(1, 10, "Performer", Day.AddHours(1), Day.AddHours(2)),
                                   RemoteBroadcast(7, 10, "Performer", Day.AddHours(3), Day.AddHours(4))));

        var (response, result) = await App.CreateClientFor(user)
                                          .GETAsync<GetBroadcasts, List<BroadcastResponse>>();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Select(b => b.BroadcastId).ShouldBe([7, 1]);
        (await App.GetBroadcastsAsync()).Select(b => b.AzuraCastBroadcastId).Order().ShouldBe([1, 7]);
    }

    [Fact]
    public async Task Stored_broadcasts_are_returned_when_AzuraCast_is_unavailable()
    {
        var user = await App.CreateUserAsync("performer", streamerId: 10);
        await App.AddBroadcastsAsync(LocalBroadcast(1, 10, "Performer", Day.AddHours(1), Day.AddHours(2)));
        A.CallTo(() => AzuraCast.GetBroadcastsAsync(A<int?>._))
         .Returns(Error<IReadOnlyCollection<Adapter.AzuraCast.ApiSchema.StationStreamerBroadcast>>(
                      HttpStatusCode.ServiceUnavailable));

        var (response, result) = await App.CreateClientFor(user)
                                          .GETAsync<GetBroadcasts, List<BroadcastResponse>>();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldHaveSingleItem().BroadcastId.ShouldBe(1);
    }
}
