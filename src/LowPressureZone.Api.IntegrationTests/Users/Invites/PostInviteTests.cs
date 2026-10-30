using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.IntegrationTests.Infrastructure;
using LowPressureZone.Data;
using LowPressureZone.Scheduling.Domain.CommunityAggregate.RelationshipEntity;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Entities;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.IntegrationTests.Users.Invites;

public sealed class PostInviteTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Admin_invites_user_to_community()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var community = await App.AddCommunityAsync("Community");

        var response = await App.CreateClientFor(admin).PostAsJsonAsync(
            "/api/users/invites",
            new { email = "new-member@example.com", communityId = community.Id, isPerformer = true, isOrganizer = false },
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        await using var scope = App.Services.CreateAsyncScope();
        var identity = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        var invitedUser = await identity.Users.SingleAsync(
            user => user.Email == "new-member@example.com", TestContext.Current.CancellationToken);
        invitedUser.LockoutEnabled.ShouldBeTrue();
        invitedUser.LockoutEnd.ShouldNotBeNull();
        invitedUser.LockoutEnd.Value.Year.ShouldBe(9999);
        var invitation = await identity.Invitations.SingleAsync(
            item => item.UserId == invitedUser.Id, TestContext.Current.CancellationToken);
        invitation.IsRegistered.ShouldBeFalse();
        var relationship = await scope.ServiceProvider.GetRequiredService<DataContext>()
                                 .Relationships.SingleAsync(item => item.UserId == invitedUser.Id,
                                                            TestContext.Current.CancellationToken);
        relationship.CommunityId.ShouldBe(community.Id);
        relationship.IsPerformer.ShouldBeTrue();
        relationship.IsOrganizer.ShouldBeFalse();
    }

    [Fact]
    public async Task Missing_community_is_not_found()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);

        var response = await App.CreateClientFor(admin).PostAsJsonAsync(
            "/api/users/invites",
            new { email = "new-member@example.com", communityId = Guid.NewGuid(), isPerformer = true, isOrganizer = false },
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var response = await App.CreateAnonymousClient().PostAsJsonAsync(
            "/api/users/invites",
            new { email = "new-member@example.com", communityId = Guid.NewGuid(), isPerformer = true, isOrganizer = false },
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
