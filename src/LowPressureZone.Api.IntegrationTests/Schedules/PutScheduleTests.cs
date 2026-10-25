using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Schedules;

public sealed class PutScheduleTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Organizer_updates_own_community_schedule()
    {
        var organizer = await App.CreateUserAsync("organizer");
        var community = await App.AddCommunityAsync("Community",
                                                    configure: c => c.SetRolesForUser(organizer.Id, false, true));
        var schedule = await App.AddScheduleAsync(community.Id);
        var request = ScheduleTestData.Request(community.Id, "Updated", isVisibleToPublic: true);

        var response = await App.CreateClientFor(organizer)
                                .PutAsJsonAsync(ScheduleTestData.Route(schedule.Id), request,
                                                TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var updated = (await App.GetSchedulesAsync()).ShouldHaveSingleItem();
        updated.Name.ShouldBe("Updated");
        updated.IsVisibleToPublic.ShouldBeTrue();
    }

    [Fact]
    public async Task Admin_updates_schedule()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var community = await App.AddCommunityAsync("Community");
        var schedule = await App.AddScheduleAsync(community.Id);

        var response = await App.CreateClientFor(admin)
                                .PutAsJsonAsync(ScheduleTestData.Route(schedule.Id),
                                                ScheduleTestData.Request(community.Id, "Updated"),
                                                TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await App.GetSchedulesAsync()).ShouldHaveSingleItem().Name.ShouldBe("Updated");
    }

    [Fact]
    public async Task Organizer_of_another_community_cannot_edit_schedule()
    {
        var organizer = await App.CreateUserAsync("organizer");
        await App.AddCommunityAsync("Other", configure: c => c.SetRolesForUser(organizer.Id, false, true));
        var community = await App.AddCommunityAsync("Target");
        var schedule = await App.AddScheduleAsync(community.Id);

        var response = await App.CreateClientFor(organizer)
                                .PutAsJsonAsync(ScheduleTestData.Route(schedule.Id),
                                                ScheduleTestData.Request(community.Id, "Changed"),
                                                TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await App.GetSchedulesAsync()).ShouldHaveSingleItem().Name.ShouldBe("Schedule");
    }

    [Fact]
    public async Task Unknown_schedule_is_not_found()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var community = await App.AddCommunityAsync("Community");

        var response = await App.CreateClientFor(admin)
                                .PutAsJsonAsync(ScheduleTestData.Route(Guid.NewGuid()),
                                                ScheduleTestData.Request(community.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Invalid_request_is_a_bad_request_and_changes_nothing()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var community = await App.AddCommunityAsync("Community");
        var schedule = await App.AddScheduleAsync(community.Id);

        var response = await App.CreateClientFor(admin)
                                .PutAsJsonAsync(ScheduleTestData.Route(schedule.Id),
                                                ScheduleTestData.Request(community.Id, "",
                                                                         isHourlyAllowed: false,
                                                                         isClashAllowed: true),
                                                TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))
            .ShouldContain("Required");
        (await App.GetSchedulesAsync()).ShouldHaveSingleItem().Name.ShouldBe("Schedule");
    }
}
