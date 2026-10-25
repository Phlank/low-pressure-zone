using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Schedules.ClashSlots;

public sealed class PutClashSlotTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Community_organizer_updates_clash_slot()
    {
        var organizer = await App.CreateUserAsync("organizer");
        var community = await App.AddCommunityAsync("Community",
                                                    configure: c => c.SetRolesForUser(organizer.Id, false, true));
        var schedule = await App.AddScheduleAsync(community.Id);
        var one = await App.AddPerformerAsync(organizer.Id, "One");
        var two = await App.AddPerformerAsync(organizer.Id, "Two");
        var slot = await App.AddClashSlotAsync(schedule.Id, one.Id, two.Id,
                                               schedule.TimeRange.StartsAt.AddHours(1), rounds: ["1", "2", "3"]);

        var response = await App.CreateClientFor(organizer)
                                .PutAsJsonAsync(ClashSlotTestData.Route(schedule.Id, slot.Id),
                                                ClashSlotTestData.Request(two.Id, one.Id,
                                                                          schedule.TimeRange.StartsAt.AddHours(2),
                                                                          rounds: ["Semi", "Final", "Third"]),
                                                TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var updated = (await App.GetSchedulesAsync()).ShouldHaveSingleItem().ClashSlots.ShouldHaveSingleItem();
        updated.PerformerOneId.ShouldBe(two.Id);
        updated.PerformerTwoId.ShouldBe(one.Id);
        updated.Rounds.ShouldBe(["Semi", "Final", "Third"]);
    }

    [Fact]
    public async Task Organizer_of_another_community_cannot_update_clash_slot()
    {
        var organizer = await App.CreateUserAsync("organizer");
        await App.AddCommunityAsync("Other", configure: c => c.SetRolesForUser(organizer.Id, false, true));
        var community = await App.AddCommunityAsync("Target");
        var schedule = await App.AddScheduleAsync(community.Id);
        var one = await App.AddPerformerAsync(organizer.Id, "One");
        var two = await App.AddPerformerAsync(organizer.Id, "Two");
        var slot = await App.AddClashSlotAsync(schedule.Id, one.Id, two.Id,
                                               schedule.TimeRange.StartsAt.AddHours(1), rounds: ["1", "2", "3"]);

        var response = await App.CreateClientFor(organizer)
                                .PutAsJsonAsync(ClashSlotTestData.Route(schedule.Id, slot.Id),
                                                ClashSlotTestData.Request(one.Id, two.Id,
                                                                          schedule.TimeRange.StartsAt.AddHours(2),
                                                                          rounds: ["Changed", "2", "3"]),
                                                TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await App.GetSchedulesAsync()).ShouldHaveSingleItem().ClashSlots.ShouldHaveSingleItem()
            .Rounds.ShouldBe(["1", "2", "3"]);
    }

    [Fact]
    public async Task Unknown_clash_slot_is_not_found()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var community = await App.AddCommunityAsync("Community");
        var schedule = await App.AddScheduleAsync(community.Id);

        var response = await App.CreateClientFor(admin)
                                .PutAsJsonAsync(ClashSlotTestData.Route(schedule.Id, Guid.NewGuid()),
                                                ClashSlotTestData.Request(Guid.NewGuid(), Guid.NewGuid(),
                                                                          schedule.TimeRange.StartsAt,
                                                                          rounds: ["1", "2", "3"]),
                                                TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
