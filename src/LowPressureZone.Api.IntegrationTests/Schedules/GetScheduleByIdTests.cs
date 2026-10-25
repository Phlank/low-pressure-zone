using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.Schedules;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Schedules;

public sealed class GetScheduleByIdTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Community_organizer_reads_private_schedule_with_permission_flags()
    {
        var organizer = await App.CreateUserAsync("organizer");
        var community = await App.AddCommunityAsync("Community",
                                                    configure: c => c.SetRolesForUser(organizer.Id, false, true));
        var schedule = await App.AddScheduleAsync(community.Id, "Private");

        var response = await App.CreateClientFor(organizer)
                                .GetAsync(ScheduleTestData.Route(schedule.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ScheduleResponse>(TestContext.Current.CancellationToken);
        body.ShouldNotBeNull();
        body.Id.ShouldBe(schedule.Id);
        body.Name.ShouldBe("Private");
        body.Community.Id.ShouldBe(community.Id);
        body.IsEditable.ShouldBeTrue();
        body.IsDeletable.ShouldBeTrue();
        body.Slots.ShouldBeEmpty();
    }

    [Fact]
    public async Task Anonymous_user_can_read_public_schedule()
    {
        var community = await App.AddCommunityAsync("Community");
        var schedule = await App.AddScheduleAsync(community.Id, isVisibleToPublic: true);

        var response = await App.CreateAnonymousClient()
                                .GetAsync(ScheduleTestData.Route(schedule.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Anonymous_user_cannot_read_private_schedule()
    {
        var community = await App.AddCommunityAsync("Community");
        var schedule = await App.AddScheduleAsync(community.Id, isVisibleToPublic: false);

        var response = await App.CreateAnonymousClient()
                                .GetAsync(ScheduleTestData.Route(schedule.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Unknown_schedule_is_not_found()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);

        var response = await App.CreateClientFor(admin)
                                .GetAsync(ScheduleTestData.Route(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
