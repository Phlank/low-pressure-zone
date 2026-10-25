using System.Net;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Communities;

public sealed class DeleteCommunityTests(ApiFixture app) : ApiTestBase(app)
{
    private static string Route(Guid id) => $"/api/communities/{id}";

    private async Task<HttpResponseMessage> DeleteAsAdminAsync(Guid id)
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        return await App.CreateClientFor(admin).DeleteAsync(Route(id), TestContext.Current.CancellationToken);
    }

    private async Task<bool> IsDeletedAsync(Guid id) => (await App.GetCommunitiesAsync()).Single(c => c.Id == id).IsDeleted;

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var community = await App.AddCommunityAsync("Alpha");

        var response = await App.CreateAnonymousClient()
                                .DeleteAsync(Route(community.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await IsDeletedAsync(community.Id)).ShouldBeFalse();
    }

    [Fact]
    public async Task Organizer_of_the_community_is_forbidden()
    {
        var organizer = await App.CreateUserAsync("organizer");
        var community = await App.AddCommunityAsync("Alpha", configure: c => c.SetRolesForUser(organizer.Id, false, true));

        var response = await App.CreateClientFor(organizer)
                                .DeleteAsync(Route(community.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await IsDeletedAsync(community.Id)).ShouldBeFalse();
    }

    [Fact]
    public async Task Admin_soft_deletes_community()
    {
        var member = await App.CreateUserAsync("member");
        var community = await App.AddCommunityAsync("Alpha", configure: c => c.SetRolesForUser(member.Id, true, false));

        var response = await DeleteAsAdminAsync(community.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var deleted = (await App.GetCommunitiesAsync()).ShouldHaveSingleItem();
        deleted.IsDeleted.ShouldBeTrue();
        deleted.Relationships.ShouldHaveSingleItem().UserId.ShouldBe(member.Id);
    }

    [Fact]
    public async Task Deleted_community_is_hidden_from_the_api()
    {
        var community = await App.AddCommunityAsync("Alpha");
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var client = App.CreateClientFor(admin);

        await client.DeleteAsync(Route(community.Id), TestContext.Current.CancellationToken);

        (await client.GetAsync(Route(community.Id), TestContext.Current.CancellationToken))
            .StatusCode.ShouldBe(HttpStatusCode.NotFound);
        (await client.GetStringAsync("/api/communities", TestContext.Current.CancellationToken)).ShouldBe("[]");
    }

    [Fact]
    public async Task Unknown_community_is_not_found()
    {
        var response = await DeleteAsAdminAsync(Guid.NewGuid());

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Already_deleted_community_is_not_found()
    {
        var community = await App.AddCommunityAsync("Alpha", configure: c => c.Delete());

        var response = await DeleteAsAdminAsync(community.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
