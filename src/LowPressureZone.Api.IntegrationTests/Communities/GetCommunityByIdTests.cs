using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.Communities;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Communities;

public sealed class GetCommunityByIdTests(ApiFixture app) : ApiTestBase(app)
{
    private static string Route(Guid id) => $"/api/communities/{id}";

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var community = await App.AddCommunityAsync("Alpha");

        var response = await App.CreateAnonymousClient()
                                .GetAsync(Route(community.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Returns_community_with_admin_permissions()
    {
        var community = await App.AddCommunityAsync("Alpha", "https://alpha.example.com");
        var admin = await App.CreateUserAsync("admin", isAdmin: true);

        var response = await App.CreateClientFor(admin)
                                .GetAsync(Route(community.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<CommunityResponse>(TestContext.Current.CancellationToken);
        body.ShouldNotBeNull();
        body.Id.ShouldBe(community.Id);
        body.Name.ShouldBe("Alpha");
        body.SocialUrl.ShouldBe("https://alpha.example.com");
        body.IsPerformable.ShouldBeTrue();
        body.IsOrganizable.ShouldBeTrue();
        body.IsEditable.ShouldBeTrue();
        body.IsDeletable.ShouldBeTrue();
    }

    [Fact]
    public async Task Permissions_reflect_the_users_relationship_with_the_community()
    {
        var user = await App.CreateUserAsync("user");
        var community = await App.AddCommunityAsync("Alpha", configure: c => c.SetRolesForUser(user.Id, true, true));

        var response = await App.CreateClientFor(user)
                                .GetAsync(Route(community.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<CommunityResponse>(TestContext.Current.CancellationToken);
        body.ShouldNotBeNull();
        body.IsPerformable.ShouldBeTrue();
        body.IsOrganizable.ShouldBeTrue();
        body.IsEditable.ShouldBeFalse();
        body.IsDeletable.ShouldBeFalse();
    }

    [Fact]
    public async Task Unknown_community_is_not_found()
    {
        var user = await App.CreateUserAsync("user");

        var response = await App.CreateClientFor(user)
                                .GetAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Deleted_community_is_not_found()
    {
        var community = await App.AddCommunityAsync("Alpha", configure: c => c.Delete());
        var admin = await App.CreateUserAsync("admin", isAdmin: true);

        var response = await App.CreateClientFor(admin)
                                .GetAsync(Route(community.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
