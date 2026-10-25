using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.Communities.Relationships;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Communities.Relationships;

public sealed class GetRelationshipsTests(ApiFixture app) : ApiTestBase(app)
{
    private static string Route(Guid communityId) => $"/api/communities/{communityId}/relationships";

    private static async Task<List<RelationshipResponse>> ReadAsync(HttpResponseMessage response)
    {
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<List<RelationshipResponse>>(
                    TestContext.Current.CancellationToken))!;
    }

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var community = await App.AddCommunityAsync("Alpha");

        var response = await App.CreateAnonymousClient()
                                .GetAsync(Route(community.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Returns_relationships_of_the_community_ordered_by_display_name()
    {
        var zed = await App.CreateUserAsync("Zed");
        var amy = await App.CreateUserAsync("Amy");
        var outsider = await App.CreateUserAsync("Outsider");
        var community = await App.AddCommunityAsync("Alpha", configure: c =>
        {
            c.SetRolesForUser(zed.Id, true, false);
            c.SetRolesForUser(amy.Id, false, true);
        });
        await App.AddCommunityAsync("Other", configure: c => c.SetRolesForUser(outsider.Id, true, true));
        var admin = await App.CreateUserAsync("admin", isAdmin: true);

        var relationships = await ReadAsync(await App.CreateClientFor(admin)
                                                     .GetAsync(Route(community.Id),
                                                               TestContext.Current.CancellationToken));

        relationships.Select(r => r.DisplayName).ShouldBe(["Amy", "Zed"]);
        relationships.ShouldAllBe(r => r.CommunityId == community.Id);
        relationships[0].UserId.ShouldBe(amy.Id);
        relationships[0].IsOrganizer.ShouldBeTrue();
        relationships[0].IsPerformer.ShouldBeFalse();
        relationships[1].UserId.ShouldBe(zed.Id);
        relationships[1].IsOrganizer.ShouldBeFalse();
        relationships[1].IsPerformer.ShouldBeTrue();
    }

    [Fact]
    public async Task Relationships_of_users_that_no_longer_exist_are_omitted()
    {
        var member = await App.CreateUserAsync("member");
        var community = await App.AddCommunityAsync("Alpha", configure: c =>
        {
            c.SetRolesForUser(member.Id, true, false);
            c.SetRolesForUser(Guid.NewGuid(), true, false);
        });

        var relationships = await ReadAsync(await App.CreateClientFor(member)
                                                     .GetAsync(Route(community.Id),
                                                               TestContext.Current.CancellationToken));

        relationships.ShouldHaveSingleItem().UserId.ShouldBe(member.Id);
    }

    [Fact]
    public async Task Admin_can_edit_every_relationship()
    {
        var member = await App.CreateUserAsync("member");
        var community = await App.AddCommunityAsync("Alpha", configure: c => c.SetRolesForUser(member.Id, true, false));
        var admin = await App.CreateUserAsync("admin", isAdmin: true);

        var relationships = await ReadAsync(await App.CreateClientFor(admin)
                                                     .GetAsync(Route(community.Id),
                                                               TestContext.Current.CancellationToken));

        relationships.ShouldHaveSingleItem().IsEditable.ShouldBeTrue();
    }

    [Fact]
    public async Task Organizer_of_the_community_can_edit_every_relationship()
    {
        var organizer = await App.CreateUserAsync("organizer");
        var member = await App.CreateUserAsync("member");
        var community = await App.AddCommunityAsync("Alpha", configure: c =>
        {
            c.SetRolesForUser(organizer.Id, false, true);
            c.SetRolesForUser(member.Id, true, false);
        });

        var relationships = await ReadAsync(await App.CreateClientFor(organizer)
                                                     .GetAsync(Route(community.Id),
                                                               TestContext.Current.CancellationToken));

        relationships.Count.ShouldBe(2);
        relationships.ShouldAllBe(r => r.IsEditable);
    }

    [Fact]
    public async Task Performer_and_unrelated_users_cannot_edit_relationships()
    {
        var performer = await App.CreateUserAsync("performer");
        var unrelated = await App.CreateUserAsync("unrelated");
        await App.MakeOrganizerAsync(unrelated);
        var community = await App.AddCommunityAsync("Alpha", configure: c => c.SetRolesForUser(performer.Id, true, false));

        foreach (var user in new[] { performer, unrelated })
        {
            var relationships = await ReadAsync(await App.CreateClientFor(user)
                                                         .GetAsync(Route(community.Id),
                                                                   TestContext.Current.CancellationToken));
            relationships.ShouldHaveSingleItem().IsEditable.ShouldBeFalse();
        }
    }

    [Fact]
    public async Task Unknown_community_returns_no_relationships()
    {
        var user = await App.CreateUserAsync("user");

        var relationships = await ReadAsync(await App.CreateClientFor(user)
                                                     .GetAsync(Route(Guid.NewGuid()),
                                                               TestContext.Current.CancellationToken));

        relationships.ShouldBeEmpty();
    }
}
