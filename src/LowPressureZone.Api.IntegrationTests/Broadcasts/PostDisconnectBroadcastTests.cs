using System.Net;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Api.Endpoints.Broadcasts.Disconnect;
using LowPressureZone.Api.IntegrationTests.Infrastructure;
using LowPressureZone.Identity.Application.Jobs;
using Quartz;
using static LowPressureZone.Api.IntegrationTests.Infrastructure.TestData;

namespace LowPressureZone.Api.IntegrationTests.Broadcasts;

public sealed class PostDisconnectBroadcastTests(ApiFixture app) : ApiTestBase(app)
{
    private const int LiveStreamerId = 30;
    private static readonly DateTime Start = new(2026, 3, 14, 20, 0, 0, DateTimeKind.Utc);

    private void SetUpAzuraCastWithLiveBroadcast()
    {
        A.CallTo(() => AzuraCast.GetBroadcastsAsync(A<int?>._))
         .Returns(BroadcastsResult(RemoteBroadcast(1, 10, "Finished", Start.AddHours(-2), Start.AddHours(-1)),
                                   RemoteBroadcast(2, LiveStreamerId, "Live", Start, hasRecording: false)));
        A.CallTo(() => AzuraCast.DisableStreamerAsync(A<int>._)).Returns(Ok(true));
        A.CallTo(() => AzuraCast.PostBroadcastingAction(BroadcastingActionType.Disconnect)).Returns(Ok(true));
    }

    private async Task<HttpResponseMessage> PostAsAdminAsync(PostDisconnectBroadcastRequest request)
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        return await App.CreateClientFor(admin)
                        .POSTAsync<PostDisconnectBroadcast, PostDisconnectBroadcastRequest>(request);
    }

    private async Task<IReadOnlyCollection<JobKey>> ScheduledJobs()
        => await (await App.Scheduler).GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var response = await App.CreateAnonymousClient()
                                .POSTAsync<PostDisconnectBroadcast, PostDisconnectBroadcastRequest>(new());

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Performer_is_forbidden()
    {
        var performer = await App.CreateUserAsync("performer", streamerId: LiveStreamerId);
        SetUpAzuraCastWithLiveBroadcast();

        var response = await App.CreateClientFor(performer)
                                .POSTAsync<PostDisconnectBroadcast, PostDisconnectBroadcastRequest>(new());

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        A.CallTo(() => AzuraCast.PostBroadcastingAction(A<BroadcastingActionType>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Admin_disconnects_current_broadcast()
    {
        SetUpAzuraCastWithLiveBroadcast();

        var response = await PostAsAdminAsync(new PostDisconnectBroadcastRequest());

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        A.CallTo(() => AzuraCast.PostBroadcastingAction(BroadcastingActionType.Disconnect))
         .MustHaveHappenedOnceExactly();
        A.CallTo(() => AzuraCast.DisableStreamerAsync(A<int>._)).MustNotHaveHappened();
        (await ScheduledJobs()).ShouldBeEmpty();
    }

    [Fact]
    public async Task Organizer_disconnects_current_broadcast()
    {
        var organizer = await App.CreateUserAsync("organizer");
        await App.MakeOrganizerAsync(organizer);
        SetUpAzuraCastWithLiveBroadcast();

        var response = await App.CreateClientFor(organizer)
                                .POSTAsync<PostDisconnectBroadcast, PostDisconnectBroadcastRequest>(new());

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        A.CallTo(() => AzuraCast.PostBroadcastingAction(BroadcastingActionType.Disconnect))
         .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Zero_disable_minutes_disables_live_streamer_without_scheduling_reenable()
    {
        SetUpAzuraCastWithLiveBroadcast();

        var response = await PostAsAdminAsync(new PostDisconnectBroadcastRequest { DisableMinutes = 0 });

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        A.CallTo(() => AzuraCast.DisableStreamerAsync(LiveStreamerId)).MustHaveHappenedOnceExactly()
         .Then(A.CallTo(() => AzuraCast.PostBroadcastingAction(BroadcastingActionType.Disconnect))
                .MustHaveHappenedOnceExactly());
        (await ScheduledJobs()).ShouldBeEmpty();
    }

    [Fact]
    public async Task Disable_minutes_schedules_live_streamer_reenable()
    {
        var streamer = await App.CreateUserAsync("streamer", streamerId: LiveStreamerId);
        SetUpAzuraCastWithLiveBroadcast();
        var requestedAt = DateTimeOffset.UtcNow;

        var response = await PostAsAdminAsync(new PostDisconnectBroadcastRequest { DisableMinutes = 15 });

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        A.CallTo(() => AzuraCast.DisableStreamerAsync(LiveStreamerId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => AzuraCast.PostBroadcastingAction(BroadcastingActionType.Disconnect))
         .MustHaveHappenedOnceExactly();

        var scheduler = await App.Scheduler;
        var jobKey = new JobKey(EnableStreamerJob.JobKey(streamer.Id));
        (await ScheduledJobs()).ShouldHaveSingleItem().ShouldBe(jobKey);
        var trigger = (await scheduler.GetTriggersOfJob(jobKey, TestContext.Current.CancellationToken))
            .ShouldHaveSingleItem();
        trigger.StartTimeUtc.ShouldBeInRange(requestedAt.AddMinutes(15), DateTimeOffset.UtcNow.AddMinutes(15));
    }

    [Fact]
    public async Task Disable_minutes_without_live_broadcast_is_a_bad_request()
    {
        SetUpAzuraCastWithLiveBroadcast();
        A.CallTo(() => AzuraCast.GetBroadcastsAsync(A<int?>._))
         .Returns(BroadcastsResult(RemoteBroadcast(1, 10, "Finished", Start.AddHours(-2), Start.AddHours(-1))));

        var response = await PostAsAdminAsync(new PostDisconnectBroadcastRequest { DisableMinutes = 15 });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))
            .ShouldContain("No broadcast is currently active.");
        A.CallTo(() => AzuraCast.DisableStreamerAsync(A<int>._)).MustNotHaveHappened();
        A.CallTo(() => AzuraCast.PostBroadcastingAction(A<BroadcastingActionType>._)).MustNotHaveHappened();
        (await ScheduledJobs()).ShouldBeEmpty();
    }

    [Fact]
    public async Task Failed_streamer_disable_is_a_bad_request()
    {
        SetUpAzuraCastWithLiveBroadcast();
        A.CallTo(() => AzuraCast.DisableStreamerAsync(LiveStreamerId))
         .Returns(Error<bool>(HttpStatusCode.InternalServerError));

        var response = await PostAsAdminAsync(new PostDisconnectBroadcastRequest { DisableMinutes = 15 });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))
            .ShouldContain("Error disabling streamer");
        A.CallTo(() => AzuraCast.PostBroadcastingAction(A<BroadcastingActionType>._)).MustNotHaveHappened();
        (await ScheduledJobs()).ShouldBeEmpty();
    }

    [Fact]
    public async Task Failed_AzuraCast_disconnect_is_a_bad_request()
    {
        SetUpAzuraCastWithLiveBroadcast();
        A.CallTo(() => AzuraCast.PostBroadcastingAction(BroadcastingActionType.Disconnect))
         .Returns(Error<bool>(HttpStatusCode.BadGateway));

        var response = await PostAsAdminAsync(new PostDisconnectBroadcastRequest());

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))
            .ShouldContain("Error returned from AzuraCast API");
    }
}
