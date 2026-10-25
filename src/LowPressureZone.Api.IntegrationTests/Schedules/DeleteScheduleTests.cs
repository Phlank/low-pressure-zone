using System.Net;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Schedules;

public sealed class DeleteScheduleTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Organizer_deletes_schedule_in_organized_community()
    {
        var organizer = await App.CreateUserAsync("organizer");
        var community = await App.AddCommunityAsync("Community",
                                                    configure: c => c.SetRolesForUser(organizer.Id, false, true));
        var schedule = await App.AddScheduleAsync(community.Id);

        var response = await App.CreateClientFor(organizer)
                                .DeleteAsync(ScheduleTestData.Route(schedule.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await App.GetSchedulesAsync()).ShouldBeEmpty();
    }

    [Fact]
    public async Task Admin_deletes_schedule()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var community = await App.AddCommunityAsync("Community");
        var schedule = await App.AddScheduleAsync(community.Id);

        var response = await App.CreateClientFor(admin)
                                .DeleteAsync(ScheduleTestData.Route(schedule.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await App.GetSchedulesAsync()).ShouldBeEmpty();
    }

    [Fact]
    public async Task Organizer_of_another_community_cannot_delete_schedule()
    {
        var organizer = await App.CreateUserAsync("organizer");
        await App.AddCommunityAsync("Other", configure: c => c.SetRolesForUser(organizer.Id, false, true));
        var community = await App.AddCommunityAsync("Target");
        var schedule = await App.AddScheduleAsync(community.Id);

        var response = await App.CreateClientFor(organizer)
                                .DeleteAsync(ScheduleTestData.Route(schedule.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await App.GetSchedulesAsync()).ShouldHaveSingleItem().Id.ShouldBe(schedule.Id);
    }

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var community = await App.AddCommunityAsync("Community");
        var schedule = await App.AddScheduleAsync(community.Id);

        var response = await App.CreateAnonymousClient()
                                .DeleteAsync(ScheduleTestData.Route(schedule.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await App.GetSchedulesAsync()).ShouldHaveSingleItem();
    }

    [Fact]
    public async Task Unknown_schedule_is_not_found()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);

        var response = await App.CreateClientFor(admin)
                                .DeleteAsync(ScheduleTestData.Route(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
