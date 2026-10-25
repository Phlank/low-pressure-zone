using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.Schedules;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Schedules;

public sealed class GetSchedulesTests(ApiFixture app) : ApiTestBase(app)
{
    private const string Route = "/api/schedules";

    private static async Task<List<ScheduleResponse>> ReadAsync(HttpResponseMessage response)
    {
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<List<ScheduleResponse>>(
                    TestContext.Current.CancellationToken))!;
    }

    [Fact]
    public async Task Anonymous_user_sees_future_public_schedules_but_not_private_schedules()
    {
        var community = await App.AddCommunityAsync("Community");
        await App.AddScheduleAsync(community.Id, "Private", isVisibleToPublic: false);
        await App.AddScheduleAsync(community.Id, "Public", isVisibleToPublic: true);

        var schedules = await ReadAsync(await App.CreateAnonymousClient()
                                                 .GetAsync(Route, TestContext.Current.CancellationToken));

        schedules.Select(schedule => schedule.Name).ShouldBe(["Public"]);
    }

    [Fact]
    public async Task Organizer_sees_own_private_schedules_and_admin_can_see_all()
    {
        var organizer = await App.CreateUserAsync("organizer");
        var organizedCommunity = await App.AddCommunityAsync("Organized",
                                                              configure: c => c.SetRolesForUser(organizer.Id, false, true));
        var otherCommunity = await App.AddCommunityAsync("Other");
        var privateSchedule = await App.AddScheduleAsync(organizedCommunity.Id, "Private");
        var publicSchedule = await App.AddScheduleAsync(otherCommunity.Id, "Public", isVisibleToPublic: true);
        var admin = await App.CreateUserAsync("admin", isAdmin: true);

        var organizerSchedules = await ReadAsync(await App.CreateClientFor(organizer)
                                                          .GetAsync(Route, TestContext.Current.CancellationToken));
        var adminSchedules = await ReadAsync(await App.CreateClientFor(admin)
                                                      .GetAsync(Route, TestContext.Current.CancellationToken));

        organizerSchedules.Select(s => s.Id).ShouldBe([privateSchedule.Id, publicSchedule.Id]);
        adminSchedules.Select(s => s.Id).ShouldContain(privateSchedule.Id);
        adminSchedules.Select(s => s.Id).ShouldContain(publicSchedule.Id);
    }

    [Fact]
    public async Task Results_are_ordered_and_can_be_filtered_by_end_time()
    {
        var community = await App.AddCommunityAsync("Community");
        var now = DateTimeOffset.UtcNow;
        var first = await App.AddScheduleAsync(community.Id, "First", now.AddDays(2), now.AddDays(2).AddHours(2),
                                               isVisibleToPublic: true);
        var second = await App.AddScheduleAsync(community.Id, "Second", now.AddDays(4), now.AddDays(4).AddHours(2),
                                                isVisibleToPublic: true);
        var third = await App.AddScheduleAsync(community.Id, "Third", now.AddDays(6), now.AddDays(6).AddHours(2),
                                               isVisibleToPublic: true);
        var client = App.CreateAnonymousClient();

        var all = await ReadAsync(await client.GetAsync(Route, TestContext.Current.CancellationToken));
        var after = await ReadAsync(await client.GetAsync(
                                       $"{Route}?after={Uri.EscapeDataString(first.TimeRange.EndsAt.ToString("O"))}",
                                       TestContext.Current.CancellationToken));
        var before = await ReadAsync(await client.GetAsync(
                                        $"{Route}?before={Uri.EscapeDataString(third.TimeRange.EndsAt.ToString("O"))}",
                                        TestContext.Current.CancellationToken));

        all.Select(s => s.Id).ShouldBe([first.Id, second.Id, third.Id]);
        after.Select(s => s.Id).ShouldBe([second.Id, third.Id]);
        before.Select(s => s.Id).ShouldBe([first.Id, second.Id]);
    }

    [Fact]
    public async Task Empty_result_is_an_empty_list()
    {
        (await ReadAsync(await App.CreateAnonymousClient().GetAsync(Route, TestContext.Current.CancellationToken)))
            .ShouldBeEmpty();
    }
}
