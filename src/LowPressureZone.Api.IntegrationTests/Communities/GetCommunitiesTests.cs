using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.Communities;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Communities;

public sealed class GetCommunitiesTests(ApiFixture app) : ApiTestBase(app)
{
    private const string Route = "/api/communities";

    private static async Task<List<CommunityResponse>> ReadCommunitiesAsync(HttpResponseMessage response)
    {
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<List<CommunityResponse>>(TestContext.Current.CancellationToken))!;
    }

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var response = await App.CreateAnonymousClient().GetAsync(Route, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Returns_communities_ordered_by_name_excluding_deleted()
    {
        await App.AddCommunityAsync("Charlie");
        await App.AddCommunityAsync("Alpha");
        await App.AddCommunityAsync("Deleted", configure: community => community.Delete());
        await App.AddCommunityAsync("Bravo");
        var user = await App.CreateUserAsync("user");

        var communities = await ReadCommunitiesAsync(
            await App.CreateClientFor(user).GetAsync(Route, TestContext.Current.CancellationToken));

        communities.Select(c => c.Name).ShouldBe(["Alpha", "Bravo", "Charlie"]);
    }

    [Fact]
    public async Task Admin_has_every_permission()
    {
        var community = await App.AddCommunityAsync("Alpha", "https://alpha.example.com");
        var admin = await App.CreateUserAsync("admin", isAdmin: true);

        var response = (await ReadCommunitiesAsync(
                            await App.CreateClientFor(admin).GetAsync(Route, TestContext.Current.CancellationToken)))
            .ShouldHaveSingleItem();

        response.Id.ShouldBe(community.Id);
        response.Name.ShouldBe("Alpha");
        response.SocialUrl.ShouldBe("https://alpha.example.com");
        response.IsPerformable.ShouldBeTrue();
        response.IsOrganizable.ShouldBeTrue();
        response.IsEditable.ShouldBeTrue();
        response.IsDeletable.ShouldBeTrue();
    }

    [Fact]
    public async Task User_without_relationships_has_no_permissions()
    {
        await App.AddCommunityAsync("Alpha");
        var user = await App.CreateUserAsync("user");

        var response = (await ReadCommunitiesAsync(
                            await App.CreateClientFor(user).GetAsync(Route, TestContext.Current.CancellationToken)))
            .ShouldHaveSingleItem();

        response.IsPerformable.ShouldBeFalse();
        response.IsOrganizable.ShouldBeFalse();
        response.IsEditable.ShouldBeFalse();
        response.IsDeletable.ShouldBeFalse();
    }

    [Fact]
    public async Task Permissions_reflect_the_users_own_relationship_with_each_community()
    {
        var performer = await App.CreateUserAsync("performer");
        var other = await App.CreateUserAsync("other");
        await App.AddCommunityAsync("Alpha", configure: c => c.SetRolesForUser(performer.Id, true, false));
        await App.AddCommunityAsync("Bravo", configure: c => c.SetRolesForUser(performer.Id, false, true));
        await App.AddCommunityAsync("Charlie", configure: c => c.SetRolesForUser(other.Id, true, true));

        var communities = (await ReadCommunitiesAsync(
                               await App.CreateClientFor(performer)
                                        .GetAsync(Route, TestContext.Current.CancellationToken)))
            .ToDictionary(c => c.Name);

        communities["Alpha"].IsPerformable.ShouldBeTrue();
        communities["Alpha"].IsOrganizable.ShouldBeFalse();
        communities["Bravo"].IsPerformable.ShouldBeFalse();
        communities["Bravo"].IsOrganizable.ShouldBeTrue();
        communities["Charlie"].IsPerformable.ShouldBeFalse();
        communities["Charlie"].IsOrganizable.ShouldBeFalse();
        communities.Values.ShouldAllBe(c => !c.IsEditable && !c.IsDeletable);
    }
}
