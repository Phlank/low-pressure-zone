using System.Net;
using LowPressureZone.Api.IntegrationTests.Infrastructure;
using LowPressureZone.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.IntegrationTests.Users.Invites;

public sealed class ResendInviteTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Admin_can_resend_invite_by_id()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var invitedUser = await UserTestData.CreateInvitedUserAsync(App, "pending@example.com");
        DateTime lastSentAt;
        Guid invitationId;
        await using (var scope = App.Services.CreateAsyncScope())
        {
            var identity = scope.ServiceProvider.GetRequiredService<IdentityContext>();
            var invitation = await identity.Invitations.SingleAsync(
                item => item.UserId == invitedUser.Id, TestContext.Current.CancellationToken);
            invitationId = invitation.Id;
            lastSentAt = invitation.LastSentDate;
        }

        var response = await App.CreateClientFor(admin).PostAsync(
            $"/api/users/invites/resend/{invitationId}", null, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        await using var updatedScope = App.Services.CreateAsyncScope();
        var updated = await updatedScope.ServiceProvider.GetRequiredService<IdentityContext>()
                                     .Invitations.AsNoTracking()
                                     .SingleAsync(item => item.Id == invitationId,
                                                  TestContext.Current.CancellationToken);
        updated.LastSentDate.ShouldBeGreaterThanOrEqualTo(lastSentAt);
    }

    [Fact]
    public async Task Missing_invite_is_not_found()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);

        var response = await App.CreateClientFor(admin).PostAsync(
            $"/api/users/invites/resend/{Guid.NewGuid()}", null, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Anonymous_user_can_request_a_resend_without_disclosing_account_state()
    {
        var user = await UserTestData.CreateInvitedUserAsync(App, "pending@example.com");

        var response = await App.CreateAnonymousClient().GetAsync(
            $"/api/users/invites/resend?email={Uri.EscapeDataString(user.Email!)}",
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Unknown_email_gets_the_same_empty_response()
    {
        var response = await App.CreateAnonymousClient().GetAsync(
            "/api/users/invites/resend?email=unknown%40example.com", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}
