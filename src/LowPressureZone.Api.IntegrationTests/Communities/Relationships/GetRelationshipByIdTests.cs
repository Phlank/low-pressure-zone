using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.Communities.Relationships;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Communities.Relationships;

public sealed class GetRelationshipByIdTests(ApiFixture app) : ApiTestBase(app)
{
    private static string Route(Guid communityId, Guid userId)
        => $"/api/communities/{communityId}/relationships/{userId}";

    private static async Task<RelationshipResponse> ReadAsync(HttpResponseMessage response)
    {
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<RelationshipResponse>(TestContext.Current.CancellationToken))!;
    }

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var member = await App.CreateUserAsync("member");
        var community = await App.AddCommunityAsync("Alpha", configure: c => c.SetRolesForUser(member.Id, true, false));

        var response = await App.CreateAnonymousClient()
                                .GetAsync(Route(community.Id, member.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Returns_the_relationship()
    {
        var member = await App.CreateUserAsync("member");
        var community = await App.AddCommunityAsync("Alpha", configure: c => c.SetRolesForUser(member.Id, true, false));

        var relationship = await ReadAsync(await App.CreateClientFor(member)
                                                    .GetAsync(Route(community.Id, member.Id),
                                                              TestContext.Current.CancellationToken));

        relationship.CommunityId.ShouldBe(community.Id);
        relationship.UserId.ShouldBe(member.Id);
        relationship.DisplayName.ShouldBe("member");
        relationship.IsPerformer.ShouldBeTrue();
        relationship.IsOrganizer.ShouldBeFalse();
        relationship.IsEditable.ShouldBeFalse();
    }

    [Fact]
    public async Task Organizer_of_the_community_can_edit_the_relationship()
    {
        var organizer = await App.CreateUserAsync("organizer");
        var member = await App.CreateUserAsync("member");
        var community = await App.AddCommunityAsync("Alpha", configure: c =>
        {
            c.SetRolesForUser(organizer.Id, false, true);
            c.SetRolesForUser(member.Id, true, false);
        });

        var relationship = await ReadAsync(await App.CreateClientFor(organizer)
                                                    .GetAsync(Route(community.Id, member.Id),
                                                              TestContext.Current.CancellationToken));

        relationship.IsEditable.ShouldBeTrue();
    }

    [Fact]
    public async Task Admin_can_edit_the_relationship()
    {
        var member = await App.CreateUserAsync("member");
        var community = await App.AddCommunityAsync("Alpha", configure: c => c.SetRolesForUser(member.Id, true, false));
        var admin = await App.CreateUserAsync("admin", isAdmin: true);

        var relationship = await ReadAsync(await App.CreateClientFor(admin)
                                                    .GetAsync(Route(community.Id, member.Id),
                                                              TestContext.Current.CancellationToken));

        relationship.IsEditable.ShouldBeTrue();
    }

    [Fact]
    public async Task Missing_relationship_is_not_found()
    {
        var member = await App.CreateUserAsync("member");
        var community = await App.AddCommunityAsync("Alpha");

        var response = await App.CreateClientFor(member)
                                .GetAsync(Route(community.Id, member.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Relationship_of_a_user_that_no_longer_exists_is_not_found()
    {
        var formerUserId = Guid.NewGuid();
        var community = await App.AddCommunityAsync("Alpha", configure: c => c.SetRolesForUser(formerUserId, true, false));
        var admin = await App.CreateUserAsync("admin", isAdmin: true);

        var response = await App.CreateClientFor(admin)
                                .GetAsync(Route(community.Id, formerUserId), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
