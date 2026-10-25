using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.Schedules.ClashSlots;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Schedules.ClashSlots;

public sealed class GetClashSlotByIdTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Anonymous_user_can_read_clash_slot()
    {
        var creator = await App.CreateUserAsync("creator");
        var community = await App.AddCommunityAsync("Community");
        var schedule = await App.AddScheduleAsync(community.Id);
        var one = await App.AddPerformerAsync(creator.Id, "One");
        var two = await App.AddPerformerAsync(creator.Id, "Two");
        var slot = await App.AddClashSlotAsync(schedule.Id, one.Id, two.Id,
                                               schedule.TimeRange.StartsAt.AddHours(1),
                                               rounds: ["1", "2", "3"]);

        var response = await App.CreateAnonymousClient()
                                .GetAsync(ClashSlotTestData.Route(schedule.Id, slot.Id),
                                          TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ClashSlotResponse>(TestContext.Current.CancellationToken);
        body.ShouldNotBeNull();
        body.Id.ShouldBe(slot.Id);
        body.ScheduleId.ShouldBe(schedule.Id);
        body.Rounds.ShouldBe(["1", "2", "3"]);
        body.IsEditable.ShouldBeFalse();
        body.IsDeletable.ShouldBeFalse();
    }

    [Fact]
    public async Task Missing_clash_slot_is_not_found()
    {
        var community = await App.AddCommunityAsync("Community");
        var schedule = await App.AddScheduleAsync(community.Id);

        var response = await App.CreateAnonymousClient()
                                .GetAsync(ClashSlotTestData.Route(schedule.Id, Guid.NewGuid()),
                                          TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
