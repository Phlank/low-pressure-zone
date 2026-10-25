using System.Net;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Schedules.HourlySlots;

public sealed class DeleteHourlySlotTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Performer_owner_deletes_hourly_slot()
    {
        var owner = await App.CreateUserAsync("owner");
        var community = await App.AddCommunityAsync("Community",
                                                    configure: c => c.SetRolesForUser(owner.Id, true, false));
        var schedule = await App.AddScheduleAsync(community.Id);
        var performer = await App.AddPerformerAsync(owner.Id, "Performer");
        var slot = await App.AddHourlySlotAsync(schedule.Id, performer.Id, schedule.TimeRange.StartsAt.AddHours(1));

        var response = await App.CreateClientFor(owner)
                                .DeleteAsync(HourlySlotTestData.Route(schedule.Id, slot.Id),
                                             TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await App.GetSchedulesAsync()).ShouldHaveSingleItem().HourlySlots.ShouldBeEmpty();
    }

    [Fact]
    public async Task Other_performer_cannot_delete_hourly_slot()
    {
        var owner = await App.CreateUserAsync("owner");
        var other = await App.CreateUserAsync("other");
        var community = await App.AddCommunityAsync("Community",
                                                    configure: c => c.SetRolesForUser(other.Id, true, false));
        var schedule = await App.AddScheduleAsync(community.Id);
        var performer = await App.AddPerformerAsync(owner.Id, "Performer");
        var slot = await App.AddHourlySlotAsync(schedule.Id, performer.Id, schedule.TimeRange.StartsAt.AddHours(1));

        var response = await App.CreateClientFor(other)
                                .DeleteAsync(HourlySlotTestData.Route(schedule.Id, slot.Id),
                                             TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await App.GetSchedulesAsync()).ShouldHaveSingleItem().HourlySlots.ShouldHaveSingleItem();
    }

    [Fact]
    public async Task Organizer_of_another_community_cannot_delete_hourly_slot()
    {
        var organizer = await App.CreateUserAsync("organizer");
        await App.MakeOrganizerAsync(organizer);
        var community = await App.AddCommunityAsync("Target");
        var schedule = await App.AddScheduleAsync(community.Id);
        var owner = await App.CreateUserAsync("owner");
        var performer = await App.AddPerformerAsync(owner.Id, "Performer");
        var slot = await App.AddHourlySlotAsync(schedule.Id, performer.Id, schedule.TimeRange.StartsAt.AddHours(1));

        var response = await App.CreateClientFor(organizer)
                                .DeleteAsync(HourlySlotTestData.Route(schedule.Id, slot.Id),
                                             TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await App.GetSchedulesAsync()).ShouldHaveSingleItem().HourlySlots.ShouldHaveSingleItem();
    }

    [Fact]
    public async Task Organizer_of_the_community_can_delete_hourly_slot()
    {
        var organizer = await App.CreateUserAsync("organizer");
        var community = await App.AddCommunityAsync("Community",
                                                    configure: c => c.SetRolesForUser(organizer.Id, false, true));
        var schedule = await App.AddScheduleAsync(community.Id);
        var performer = await App.AddPerformerAsync(organizer.Id, "Performer");
        var slot = await App.AddHourlySlotAsync(schedule.Id, performer.Id, schedule.TimeRange.StartsAt.AddHours(1));

        var response = await App.CreateClientFor(organizer)
                                .DeleteAsync(HourlySlotTestData.Route(schedule.Id, slot.Id),
                                             TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await App.GetSchedulesAsync()).ShouldHaveSingleItem().HourlySlots.ShouldBeEmpty();
    }
}
