using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.Users.Invites;
using LowPressureZone.Api.IntegrationTests.Infrastructure;
using LowPressureZone.Data;
using LowPressureZone.Scheduling.Domain.CommunityAggregate.RelationshipEntity;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Entities;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.IntegrationTests.Users.Invites;

public sealed class GetInvitesTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Admin_gets_pending_invites_with_community_ids()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var community = await App.AddCommunityAsync("Community");
        var user = await UserTestData.CreateInvitedUserAsync(App, "pending@example.com");
        await using (var scope = App.Services.CreateAsyncScope())
        {
            var data = scope.ServiceProvider.GetRequiredService<DataContext>();
            var communityEntity = await data.Communities.SingleAsync(
                item => item.Id == community.Id, TestContext.Current.CancellationToken);
            communityEntity.SetRolesForUser(user.Id, true, false);
            await data.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var response = await App.CreateClientFor(admin).GetAsync(
            "/api/users/invites", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var invites = await response.Content.ReadFromJsonAsync<List<InviteResponse>>(
            TestContext.Current.CancellationToken);
        invites.ShouldNotBeNull();
        invites.Count.ShouldBe(1);
        invites[0].Email.ShouldBe("pending@example.com");
        invites[0].DisplayName.ShouldBe("pending@example.com");
        invites[0].CommunityIds.ShouldContain(community.Id);
    }

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var response = await App.CreateAnonymousClient().GetAsync(
            "/api/users/invites", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
