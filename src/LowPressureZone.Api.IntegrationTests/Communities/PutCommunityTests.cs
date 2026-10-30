using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.Communities;
using LowPressureZone.Api.IntegrationTests.Infrastructure;
using LowPressureZone.Scheduling.Domain.CommunityAggregate;

namespace LowPressureZone.Api.IntegrationTests.Communities;

public sealed class PutCommunityTests(ApiFixture app) : ApiTestBase(app)
{
    private static string Route(Guid id) => $"/api/communities/{id}";

    private static CommunityRequest Request(string name = "Renamed", string socialUrl = "https://renamed.example.com")
        => new() { Name = name, SocialUrl = socialUrl };

    private Task<Community> AddOriginalAsync(Action<Community>? configure = null)
        => App.AddCommunityAsync("Original", "https://original.example.com", configure);

    private async Task<HttpResponseMessage> PutAsAdminAsync(Guid id, CommunityRequest request)
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        return await App.CreateClientFor(admin)
                        .PutAsJsonAsync(Route(id), request, TestContext.Current.CancellationToken);
    }

    private async Task ShouldBeUnchangedAsync(Guid id)
    {
        var community = (await App.GetCommunitiesAsync()).Single(c => c.Id == id);
        community.Name.ShouldBe("Original");
        community.SocialUrl.ShouldBe("https://original.example.com");
    }

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var community = await AddOriginalAsync();

        var response = await App.CreateAnonymousClient()
                                .PutAsJsonAsync(Route(community.Id), Request(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        await ShouldBeUnchangedAsync(community.Id);
    }

    [Fact]
    public async Task Organizer_of_the_community_is_forbidden()
    {
        var organizer = await App.CreateUserAsync("organizer");
        var community = await AddOriginalAsync(c => c.SetRolesForUser(organizer.Id, false, true));

        var response = await App.CreateClientFor(organizer)
                                .PutAsJsonAsync(Route(community.Id), Request(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        await ShouldBeUnchangedAsync(community.Id);
    }

    [Fact]
    public async Task Admin_updates_community()
    {
        var community = await AddOriginalAsync();

        var response = await PutAsAdminAsync(community.Id, Request("  Renamed  ", "https://renamed.example.com"));

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var updated = (await App.GetCommunitiesAsync()).ShouldHaveSingleItem();
        updated.Name.ShouldBe("Renamed");
        updated.SocialUrl.ShouldBe("https://renamed.example.com");
    }

    [Fact]
    public async Task Unknown_community_is_not_found()
    {
        var response = await PutAsAdminAsync(Guid.NewGuid(), Request());

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Deleted_community_is_not_found()
    {
        var community = await AddOriginalAsync(c => c.Delete());

        var response = await PutAsAdminAsync(community.Id, Request());

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await ShouldBeUnchangedAsync(community.Id);
    }

    [Theory]
    [InlineData("", "https://renamed.example.com", "Required")]
    [InlineData("Renamed", "not a url", "Invalid URL")]
    public async Task Invalid_request_is_a_bad_request_and_changes_nothing(
        string name,
        string socialUrl,
        string expectedError)
    {
        var community = await AddOriginalAsync();

        var response = await PutAsAdminAsync(community.Id, Request(name, socialUrl));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).ShouldContain(expectedError);
        await ShouldBeUnchangedAsync(community.Id);
    }

    [Fact]
    public async Task Keeping_the_current_name_is_not_a_conflict()
    {
        var community = await AddOriginalAsync();

        var response = await PutAsAdminAsync(community.Id, Request("Original", "https://renamed.example.com"));

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await App.GetCommunitiesAsync()).ShouldHaveSingleItem().SocialUrl.ShouldBe("https://renamed.example.com");
    }

    [Fact]
    public async Task Name_taken_by_another_community_is_a_bad_request()
    {
        var community = await AddOriginalAsync();
        await App.AddCommunityAsync("Taken", "https://taken.example.com");

        var response = await PutAsAdminAsync(community.Id, Request("Taken"));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).ShouldContain("Name is already in use");
        await ShouldBeUnchangedAsync(community.Id);
    }
}
