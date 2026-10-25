using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Schedules.ClashSlots;

public sealed class PostClashSlotTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Community_organizer_creates_clash_slot()
    {
        var organizer = await App.CreateUserAsync("organizer");
        var community = await App.AddCommunityAsync("Community",
                                                    configure: c => c.SetRolesForUser(organizer.Id, false, true));
        var schedule = await App.AddScheduleAsync(community.Id);
        var performerOne = await App.AddPerformerAsync(organizer.Id, "One");
        var performerTwo = await App.AddPerformerAsync(organizer.Id, "Two");

        var response = await App.CreateClientFor(organizer)
                                .PostAsJsonAsync(ClashSlotTestData.Route(schedule.Id),
                                                 ClashSlotTestData.Request(performerOne.Id, performerTwo.Id,
                                                                           schedule.TimeRange.StartsAt.AddHours(1),
                                                                           rounds: ["Round 1", "Round 2", "Final"]),
                                                 TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var slot = (await App.GetSchedulesAsync()).ShouldHaveSingleItem().ClashSlots.ShouldHaveSingleItem();
        slot.PerformerOneId.ShouldBe(performerOne.Id);
        slot.PerformerTwoId.ShouldBe(performerTwo.Id);
        slot.Rounds.ShouldBe(["Round 1", "Round 2", "Final"]);
        response.Headers.Location.ShouldNotBeNull();
        response.Headers.Location.ToString().ShouldContain($"/schedules/{schedule.Id}/clash-slots/");
        var locationResponse = await App.CreateClientFor(organizer)
                                       .GetAsync(response.Headers.Location, TestContext.Current.CancellationToken);
        locationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Organizer_of_another_community_cannot_create_clash_slot()
    {
        var organizer = await App.CreateUserAsync("organizer");
        await App.AddCommunityAsync("Other", configure: c => c.SetRolesForUser(organizer.Id, false, true));
        var community = await App.AddCommunityAsync("Target");
        var schedule = await App.AddScheduleAsync(community.Id);
        var one = await App.AddPerformerAsync(organizer.Id, "One");
        var two = await App.AddPerformerAsync(organizer.Id, "Two");

        var response = await App.CreateClientFor(organizer)
                                .PostAsJsonAsync(ClashSlotTestData.Route(schedule.Id),
                                                 ClashSlotTestData.Request(one.Id, two.Id,
                                                                           schedule.TimeRange.StartsAt.AddHours(1),
                                                                           rounds: ["1", "2", "3"]),
                                                 TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await App.GetSchedulesAsync()).ShouldHaveSingleItem().ClashSlots.ShouldBeEmpty();
    }

    [Fact]
    public async Task Clash_slots_must_be_enabled_for_schedule()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var community = await App.AddCommunityAsync("Community");
        var schedule = await App.AddScheduleAsync(community.Id, isClashAllowed: false);
        var one = await App.AddPerformerAsync(admin.Id, "One");
        var two = await App.AddPerformerAsync(admin.Id, "Two");

        var response = await App.CreateClientFor(admin)
                                .PostAsJsonAsync(ClashSlotTestData.Route(schedule.Id),
                                                 ClashSlotTestData.Request(one.Id, two.Id,
                                                                           schedule.TimeRange.StartsAt.AddHours(1),
                                                                           rounds: ["1", "2", "3"]),
                                                 TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await App.GetSchedulesAsync()).ShouldHaveSingleItem().ClashSlots.ShouldBeEmpty();
    }

    [Fact]
    public async Task Unknown_schedule_is_not_found()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var one = await App.AddPerformerAsync(admin.Id, "One");
        var two = await App.AddPerformerAsync(admin.Id, "Two");

        var response = await App.CreateClientFor(admin)
                                .PostAsJsonAsync(ClashSlotTestData.Route(Guid.NewGuid()),
                                                 ClashSlotTestData.Request(one.Id, two.Id,
                                                                           DateTimeOffset.UtcNow.AddDays(2),
                                                                           rounds: ["1", "2", "3"]),
                                                 TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Request_must_have_three_rounds_and_two_hour_duration()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var community = await App.AddCommunityAsync("Community");
        var schedule = await App.AddScheduleAsync(community.Id);
        var one = await App.AddPerformerAsync(admin.Id, "One");
        var two = await App.AddPerformerAsync(admin.Id, "Two");

        var response = await App.CreateClientFor(admin)
                                .PostAsJsonAsync(ClashSlotTestData.Route(schedule.Id),
                                                 ClashSlotTestData.Request(one.Id, two.Id,
                                                                           schedule.TimeRange.StartsAt.AddHours(1),
                                                                           duration: 3, rounds: ["Only one"]),
                                                 TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))
            .ShouldContain("Must have 3 rounds");
        (await App.GetSchedulesAsync()).ShouldHaveSingleItem().ClashSlots.ShouldBeEmpty();
    }
}
