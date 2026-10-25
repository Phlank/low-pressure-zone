using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.Schedules.HourlySlots;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Schedules.HourlySlots;

public sealed class GetHourlySlotByIdTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Anonymous_user_can_read_hourly_slot()
    {
        var creator = await App.CreateUserAsync("creator");
        var community = await App.AddCommunityAsync("Community");
        var schedule = await App.AddScheduleAsync(community.Id);
        var performer = await App.AddPerformerAsync(creator.Id, "Performer");
        var slot = await App.AddHourlySlotAsync(schedule.Id, performer.Id, schedule.TimeRange.StartsAt.AddHours(1));

        var response = await App.CreateAnonymousClient()
                                .GetAsync(HourlySlotTestData.Route(schedule.Id, slot.Id),
                                          TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<HourlySlotResponse>(TestContext.Current.CancellationToken);
        body.ShouldNotBeNull();
        body.Id.ShouldBe(slot.Id);
        body.ScheduleId.ShouldBe(schedule.Id);
        body.PerformerId.ShouldBe(performer.Id);
        body.IsEditable.ShouldBeFalse();
        body.IsDeletable.ShouldBeFalse();
    }

    [Fact]
    public async Task Missing_hourly_slot_is_not_found()
    {
        var community = await App.AddCommunityAsync("Community");
        var schedule = await App.AddScheduleAsync(community.Id);

        var response = await App.CreateAnonymousClient()
                                .GetAsync(HourlySlotTestData.Route(schedule.Id, Guid.NewGuid()),
                                          TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
