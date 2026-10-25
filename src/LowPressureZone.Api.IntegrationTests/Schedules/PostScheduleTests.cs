using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.Schedules;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Schedules;

public sealed class PostScheduleTests(ApiFixture app) : ApiTestBase(app)
{
    private const string Route = "/api/schedules";

    [Fact]
    public async Task Organizer_creates_schedule_in_organized_community()
    {
        var organizer = await App.CreateUserAsync("organizer");
        var community = await App.AddCommunityAsync("Community",
                                                    configure: c => c.SetRolesForUser(organizer.Id, false, true));

        var response = await App.CreateClientFor(organizer)
                                .PostAsJsonAsync(Route, ScheduleTestData.Request(community.Id, "Schedule"),
                                                 TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var schedule = (await App.GetSchedulesAsync()).ShouldHaveSingleItem();
        schedule.Name.ShouldBe("Schedule");
        schedule.CommunityId.ShouldBe(community.Id);
        response.Headers.Location.ShouldNotBeNull();
        response.Headers.Location.ToString().ShouldEndWith($"/schedules/{schedule.Id}");

        var locationResponse = await App.CreateClientFor(organizer)
                                       .GetAsync(response.Headers.Location, TestContext.Current.CancellationToken);
        locationResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Admin_creates_schedule()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var community = await App.AddCommunityAsync("Community");

        var response = await App.CreateClientFor(admin)
                                .PostAsJsonAsync(Route, ScheduleTestData.Request(community.Id),
                                                 TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        (await App.GetSchedulesAsync()).ShouldHaveSingleItem().CommunityId.ShouldBe(community.Id);
    }

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var community = await App.AddCommunityAsync("Community");

        var response = await App.CreateAnonymousClient()
                                .PostAsJsonAsync(Route, ScheduleTestData.Request(community.Id),
                                                 TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await App.GetSchedulesAsync()).ShouldBeEmpty();
    }

    [Fact]
    public async Task Organizer_of_another_community_is_unauthorized()
    {
        var organizer = await App.CreateUserAsync("organizer");
        await App.AddCommunityAsync("Other", configure: c => c.SetRolesForUser(organizer.Id, false, true));
        var community = await App.AddCommunityAsync("Target");

        var response = await App.CreateClientFor(organizer)
                                .PostAsJsonAsync(Route, ScheduleTestData.Request(community.Id),
                                                 TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await App.GetSchedulesAsync()).ShouldBeEmpty();
    }

    [Fact]
    public async Task Unknown_community_is_not_found()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);

        var response = await App.CreateClientFor(admin)
                                .PostAsJsonAsync(Route, ScheduleTestData.Request(Guid.NewGuid()),
                                                 TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("", true, false, "Required")]
    [InlineData("Schedule", false, false, "At least one slot type must be allowed")]
    public async Task Invalid_request_is_a_bad_request(
        string name,
        bool isHourlyAllowed,
        bool isClashAllowed,
        string expectedError)
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var community = await App.AddCommunityAsync("Community");
        var request = ScheduleTestData.Request(community.Id, name, isHourlyAllowed: isHourlyAllowed,
                                               isClashAllowed: isClashAllowed);

        var response = await App.CreateClientFor(admin)
                                .PostAsJsonAsync(Route, request, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).ShouldContain(expectedError);
        (await App.GetSchedulesAsync()).ShouldBeEmpty();
    }
}
