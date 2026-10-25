using System.Net;
using LowPressureZone.Api.IntegrationTests.Infrastructure;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.IntegrationTests.Users.VerifyToken;

public sealed class GetVerifyTokenTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Valid_invitation_token_is_accepted()
    {
        var user = await UserTestData.CreateInvitedUserAsync(App, "verify@example.com");
        var context = await UserTestData.CreateInviteContextAsync(App, user.Id);

        var response = await App.CreateAnonymousClient().GetAsync(
            $"/api/users/verifytoken?context={Uri.EscapeDataString(context)}&provider=Default&purpose=Invite",
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Invalid_context_is_forbidden()
    {
        var response = await App.CreateAnonymousClient().GetAsync(
            "/api/users/verifytoken?context=invalid&provider=Default&purpose=Invite",
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Registered_invitation_cannot_be_verified_again()
    {
        var user = await UserTestData.CreateInvitedUserAsync(App, "verify@example.com");
        var context = await UserTestData.CreateInviteContextAsync(App, user.Id);
        await using (var scope = App.Services.CreateAsyncScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var registeredUser = await userManager.FindByIdAsync(user.Id.ToString());
            registeredUser.ShouldNotBeNull();
            var identity = scope.ServiceProvider.GetRequiredService<LowPressureZone.Identity.IdentityContext>();
            var invitation = await identity.Invitations.SingleAsync(
                item => item.UserId == user.Id, TestContext.Current.CancellationToken);
            invitation.IsRegistered = true;
            await identity.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var response = await App.CreateAnonymousClient().GetAsync(
            $"/api/users/verifytoken?context={Uri.EscapeDataString(context)}&provider=Default&purpose=Invite",
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
