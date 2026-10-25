using System.Net;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Schedules.HourlySlots;

public sealed class PutHourlySlotTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Performer_owner_updates_hourly_slot()
    {
        var owner = await App.CreateUserAsync("owner");
        var community = await App.AddCommunityAsync("Community",
                                                    configure: c => c.SetRolesForUser(owner.Id, true, false));
        var schedule = await App.AddScheduleAsync(community.Id);
        var performer = await App.AddPerformerAsync(owner.Id, "Performer");
        var slot = await App.AddHourlySlotAsync(schedule.Id, performer.Id, schedule.TimeRange.StartsAt.AddHours(1));
        var request = HourlySlotTestData.Request(performer.Id, schedule.TimeRange.StartsAt.AddHours(2), subtitle: "Updated");

        var response = await App.CreateClientFor(owner)
                                .PutAsync(HourlySlotTestData.Route(schedule.Id, slot.Id),
                                          HourlySlotTestData.AsForm(request), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var updated = (await App.GetSchedulesAsync()).ShouldHaveSingleItem().HourlySlots.ShouldHaveSingleItem();
        updated.Subtitle.ShouldBe("Updated");
        updated.TimeRange.StartsAt.ShouldBe(request.StartsAt, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task Other_performer_cannot_update_hourly_slot()
    {
        var owner = await App.CreateUserAsync("owner");
        var other = await App.CreateUserAsync("other");
        var community = await App.AddCommunityAsync("Community",
                                                    configure: c => c.SetRolesForUser(other.Id, true, false));
        var schedule = await App.AddScheduleAsync(community.Id);
        var performer = await App.AddPerformerAsync(owner.Id, "Performer");
        var slot = await App.AddHourlySlotAsync(schedule.Id, performer.Id, schedule.TimeRange.StartsAt.AddHours(1));

        var response = await App.CreateClientFor(other)
                                .PutAsync(HourlySlotTestData.Route(schedule.Id, slot.Id),
                                          HourlySlotTestData.AsForm(
                                              HourlySlotTestData.Request(performer.Id,
                                                                         schedule.TimeRange.StartsAt.AddHours(2),
                                                                         subtitle: "Changed")),
                                          TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await App.GetSchedulesAsync()).ShouldHaveSingleItem().HourlySlots.ShouldHaveSingleItem()
            .Subtitle.ShouldBeNull();
    }

    [Fact]
    public async Task Organizer_of_another_community_cannot_update_hourly_slot()
    {
        var organizer = await App.CreateUserAsync("organizer");
        await App.MakeOrganizerAsync(organizer);
        var community = await App.AddCommunityAsync("Target");
        var schedule = await App.AddScheduleAsync(community.Id);
        var owner = await App.CreateUserAsync("owner");
        var performer = await App.AddPerformerAsync(owner.Id, "Performer");
        var slot = await App.AddHourlySlotAsync(schedule.Id, performer.Id, schedule.TimeRange.StartsAt.AddHours(1));

        var response = await App.CreateClientFor(organizer)
                                .PutAsync(HourlySlotTestData.Route(schedule.Id, slot.Id),
                                          HourlySlotTestData.AsForm(HourlySlotTestData.Request(
                                                                        performer.Id,
                                                                        schedule.TimeRange.StartsAt.AddHours(2))),
                                          TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await App.GetSchedulesAsync()).ShouldHaveSingleItem().HourlySlots.ShouldHaveSingleItem()
            .Subtitle.ShouldBeNull();
    }

    [Fact]
    public async Task Organizer_of_the_community_can_update_hourly_slot()
    {
        var organizer = await App.CreateUserAsync("organizer");
        var community = await App.AddCommunityAsync("Community",
                                                    configure: c => c.SetRolesForUser(organizer.Id, false, true));
        var schedule = await App.AddScheduleAsync(community.Id);
        var performer = await App.AddPerformerAsync(organizer.Id, "Performer");
        var slot = await App.AddHourlySlotAsync(schedule.Id, performer.Id, schedule.TimeRange.StartsAt.AddHours(1));

        var response = await App.CreateClientFor(organizer)
                                .PutAsync(HourlySlotTestData.Route(schedule.Id, slot.Id),
                                          HourlySlotTestData.AsForm(HourlySlotTestData.Request(
                                                                        performer.Id,
                                                                        schedule.TimeRange.StartsAt.AddHours(2),
                                                                        subtitle: "Organizer edit")),
                                          TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await App.GetSchedulesAsync()).ShouldHaveSingleItem().HourlySlots.ShouldHaveSingleItem()
            .Subtitle.ShouldBe("Organizer edit");
    }

    [Fact]
    public async Task Unknown_hourly_slot_is_not_found()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var community = await App.AddCommunityAsync("Community");
        var schedule = await App.AddScheduleAsync(community.Id);

        var response = await App.CreateClientFor(admin)
                                .PutAsync(HourlySlotTestData.Route(schedule.Id, Guid.NewGuid()),
                                          HourlySlotTestData.AsForm(HourlySlotTestData.Request(
                                                                        Guid.NewGuid(),
                                                                        schedule.TimeRange.StartsAt)),
                                          TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
