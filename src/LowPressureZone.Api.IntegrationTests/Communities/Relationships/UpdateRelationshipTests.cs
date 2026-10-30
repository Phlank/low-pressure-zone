using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.Communities.Relationships;
using LowPressureZone.Api.IntegrationTests.Infrastructure;
using LowPressureZone.Scheduling.Domain.CommunityAggregate.RelationshipEntity;

namespace LowPressureZone.Api.IntegrationTests.Communities.Relationships;

public sealed class UpdateRelationshipTests(ApiFixture app) : ApiTestBase(app)
{
    private static string Route(Guid communityId, Guid userId)
        => $"/api/communities/{communityId}/relationships/{userId}";

    private static RelationshipRequest Request(bool isPerformer, bool isOrganizer)
        => new() { IsPerformer = isPerformer, IsOrganizer = isOrganizer };

    private async Task<List<Relationship>> GetRelationshipsAsync(Guid communityId)
        => (await App.GetCommunitiesAsync()).Single(c => c.Id == communityId).Relationships;

    private async Task<HttpResponseMessage> PutAsAdminAsync(Guid communityId, Guid userId, RelationshipRequest request)
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        return await App.CreateClientFor(admin)
                        .PutAsJsonAsync(Route(communityId, userId), request, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var member = await App.CreateUserAsync("member");
        var community = await App.AddCommunityAsync("Alpha");

        var response = await App.CreateAnonymousClient()
                                .PutAsJsonAsync(Route(community.Id, member.Id), Request(true, true),
                                                TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await GetRelationshipsAsync(community.Id)).ShouldBeEmpty();
    }

    [Fact]
    public async Task Performer_is_forbidden()
    {
        var performer = await App.CreateUserAsync("performer");
        var community = await App.AddCommunityAsync("Alpha", configure: c => c.SetRolesForUser(performer.Id, true, false));

        var response = await App.CreateClientFor(performer)
                                .PutAsJsonAsync(Route(community.Id, performer.Id), Request(true, true),
                                                TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await GetRelationshipsAsync(community.Id)).ShouldHaveSingleItem().IsOrganizer.ShouldBeFalse();
    }

    [Fact]
    public async Task Admin_creates_a_relationship()
    {
        var member = await App.CreateUserAsync("member");
        var community = await App.AddCommunityAsync("Alpha");

        var response = await PutAsAdminAsync(community.Id, member.Id, Request(true, false));

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var relationship = (await GetRelationshipsAsync(community.Id)).ShouldHaveSingleItem();
        relationship.UserId.ShouldBe(member.Id);
        relationship.IsPerformer.ShouldBeTrue();
        relationship.IsOrganizer.ShouldBeFalse();
    }

    [Fact]
    public async Task Admin_updates_an_existing_relationship()
    {
        var member = await App.CreateUserAsync("member");
        var community = await App.AddCommunityAsync("Alpha", configure: c => c.SetRolesForUser(member.Id, true, false));

        var response = await PutAsAdminAsync(community.Id, member.Id, Request(false, true));

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var relationship = (await GetRelationshipsAsync(community.Id)).ShouldHaveSingleItem();
        relationship.IsPerformer.ShouldBeFalse();
        relationship.IsOrganizer.ShouldBeTrue();
    }

    [Fact]
    public async Task Post_verb_also_updates_the_relationship()
    {
        var member = await App.CreateUserAsync("member");
        var community = await App.AddCommunityAsync("Alpha");
        var admin = await App.CreateUserAsync("admin", isAdmin: true);

        var response = await App.CreateClientFor(admin)
                                .PostAsJsonAsync(Route(community.Id, member.Id), Request(true, true),
                                                 TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var relationship = (await GetRelationshipsAsync(community.Id)).ShouldHaveSingleItem();
        relationship.IsPerformer.ShouldBeTrue();
        relationship.IsOrganizer.ShouldBeTrue();
    }

    [Fact]
    public async Task Organizer_of_the_community_updates_a_relationship()
    {
        var organizer = await App.CreateUserAsync("organizer");
        var member = await App.CreateUserAsync("member");
        var community = await App.AddCommunityAsync("Alpha", configure: c => c.SetRolesForUser(organizer.Id, false, true));

        var response = await App.CreateClientFor(organizer)
                                .PutAsJsonAsync(Route(community.Id, member.Id), Request(true, false),
                                                TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await GetRelationshipsAsync(community.Id)).Single(r => r.UserId == member.Id).IsPerformer.ShouldBeTrue();
    }

    [Fact]
    public async Task Organizer_of_another_community_is_forbidden()
    {
        var organizer = await App.CreateUserAsync("organizer");
        await App.MakeOrganizerAsync(organizer);
        var member = await App.CreateUserAsync("member");
        var community = await App.AddCommunityAsync("Alpha");

        var response = await App.CreateClientFor(organizer)
                                .PutAsJsonAsync(Route(community.Id, member.Id), Request(true, true),
                                                TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await GetRelationshipsAsync(community.Id)).ShouldBeEmpty();
    }

    [Fact]
    public async Task Relationship_of_an_admin_cannot_be_changed()
    {
        var otherAdmin = await App.CreateUserAsync("other-admin", isAdmin: true);
        var community = await App.AddCommunityAsync("Alpha");

        var response = await PutAsAdminAsync(community.Id, otherAdmin.Id, Request(true, true));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await GetRelationshipsAsync(community.Id)).ShouldBeEmpty();
    }

    [Fact]
    public async Task Unknown_community_is_not_found()
    {
        var member = await App.CreateUserAsync("member");

        var response = await PutAsAdminAsync(Guid.NewGuid(), member.Id, Request(true, false));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Deleted_community_is_not_found()
    {
        var member = await App.CreateUserAsync("member");
        var community = await App.AddCommunityAsync("Alpha", configure: c => c.Delete());

        var response = await PutAsAdminAsync(community.Id, member.Id, Request(true, false));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        (await GetRelationshipsAsync(community.Id)).ShouldBeEmpty();
    }

    [Fact]
    public async Task Unknown_user_is_not_found()
    {
        var community = await App.AddCommunityAsync("Alpha");

        var response = await PutAsAdminAsync(community.Id, Guid.NewGuid(), Request(true, false));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        (await GetRelationshipsAsync(community.Id)).ShouldBeEmpty();
    }
}
