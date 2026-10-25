using System.Net;
using LowPressureZone.Api.Endpoints.Schedules.HourlySlots;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Schedules.HourlySlots;

public sealed class PostHourlySlotTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Performer_creates_hourly_slot_in_schedule()
    {
        var performerUser = await App.CreateUserAsync("performer");
        var community = await App.AddCommunityAsync("Community",
                                                    configure: c => c.SetRolesForUser(performerUser.Id, true, false));
        var schedule = await App.AddScheduleAsync(community.Id);
        var performer = await App.AddPerformerAsync(performerUser.Id, "Performer");
        var request = HourlySlotTestData.Request(performer.Id, schedule.TimeRange.StartsAt.AddHours(1),
                                                 subtitle: "Show");

        var response = await App.CreateClientFor(performerUser)
                                .PostAsync(HourlySlotTestData.Route(schedule.Id), HourlySlotTestData.AsForm(request),
                                           TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Headers.Location.ShouldNotBeNull();
        response.Headers.Location.ToString().ShouldContain($"/schedules/{schedule.Id}/hourlySlots/");
        var slot = (await App.GetSchedulesAsync()).Single().HourlySlots.ShouldHaveSingleItem();
        slot.PerformerId.ShouldBe(performer.Id);
        slot.Subtitle.ShouldBe("Show");
    }

    [Fact]
    public async Task Admin_cannot_add_hourly_slot_to_schedule_when_hourly_slots_are_disabled()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var community = await App.AddCommunityAsync("Community");
        var schedule = await App.AddScheduleAsync(community.Id, isHourlyAllowed: false);
        var performer = await App.AddPerformerAsync(admin.Id, "Performer");

        var response = await App.CreateClientFor(admin)
                                .PostAsync(HourlySlotTestData.Route(schedule.Id),
                                           HourlySlotTestData.AsForm(HourlySlotTestData.Request(performer.Id, schedule.TimeRange.StartsAt)),
                                           TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await App.GetSchedulesAsync()).ShouldHaveSingleItem().HourlySlots.ShouldBeEmpty();
    }

    [Fact]
    public async Task Performer_cannot_add_another_users_performer_to_schedule()
    {
        var performerUser = await App.CreateUserAsync("performer");
        var community = await App.AddCommunityAsync("Community",
                                                    configure: c => c.SetRolesForUser(performerUser.Id, true, false));
        var schedule = await App.AddScheduleAsync(community.Id);
        var owner = await App.CreateUserAsync("owner");
        var performer = await App.AddPerformerAsync(owner.Id, "Performer");

        var response = await App.CreateClientFor(performerUser)
                                .PostAsync(HourlySlotTestData.Route(schedule.Id),
                                           HourlySlotTestData.AsForm(HourlySlotTestData.Request(performer.Id, schedule.TimeRange.StartsAt)),
                                           TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await App.GetSchedulesAsync()).ShouldHaveSingleItem().HourlySlots.ShouldBeEmpty();
    }

    [Fact]
    public async Task Unknown_schedule_is_not_found()
    {
        var performer = await App.CreateUserAsync("performer");
        await App.AddCommunityAsync("Performer community", configure: c => c.SetRolesForUser(performer.Id, true, false));

        var response = await App.CreateClientFor(performer)
                                .PostAsync(HourlySlotTestData.Route(Guid.NewGuid()),
                                           HourlySlotTestData.AsForm(HourlySlotTestData.Request(
                                                                         (await App.AddPerformerAsync(performer.Id,
                                                                                                      "Performer")).Id,
                                                                         DateTimeOffset.UtcNow.AddDays(2))),
                                           TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
