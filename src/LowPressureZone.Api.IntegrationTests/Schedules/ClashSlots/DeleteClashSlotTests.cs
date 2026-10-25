using System.Net;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Schedules.ClashSlots;

public sealed class DeleteClashSlotTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Community_organizer_deletes_clash_slot()
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
                                .DeleteAsync(ClashSlotTestData.Route(schedule.Id, slot.Id),
                                             TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await App.GetSchedulesAsync()).ShouldHaveSingleItem().ClashSlots.ShouldBeEmpty();
    }

    [Fact]
    public async Task Organizer_of_another_community_cannot_delete_clash_slot()
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
                                .DeleteAsync(ClashSlotTestData.Route(schedule.Id, slot.Id),
                                             TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await App.GetSchedulesAsync()).ShouldHaveSingleItem().ClashSlots.ShouldHaveSingleItem();
    }
}
