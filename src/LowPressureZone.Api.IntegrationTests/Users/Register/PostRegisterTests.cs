using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Api.IntegrationTests.Infrastructure;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static LowPressureZone.Api.IntegrationTests.Infrastructure.TestData;

namespace LowPressureZone.Api.IntegrationTests.Users.Register;

public sealed class PostRegisterTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Invitee_can_register_and_get_linked_to_a_new_streamer()
    {
        var user = await UserTestData.CreateInvitedUserAsync(App, "register@example.com");
        var context = await UserTestData.CreateInviteContextAsync(App, user.Id);
        A.CallTo(() => AzuraCast.PostStreamerAsync("new-user", A<string>._, "New User"))
         .Returns(Ok(87));

        var response = await App.CreateAnonymousClient().PostAsJsonAsync(
            "/api/users/register",
            new
            {
                context,
                displayName = "New User",
                username = "new-user",
                password = UserTestData.ValidPassword,
                confirmPassword = UserTestData.ValidPassword
            },
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        await using var scope = App.Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var registeredUser = await userManager.FindByIdAsync(user.Id.ToString());
        registeredUser.ShouldNotBeNull();
        registeredUser.UserName.ShouldBe("new-user");
        registeredUser.DisplayName.ShouldBe("New User");
        registeredUser.EmailConfirmed.ShouldBeTrue();
        registeredUser.TwoFactorEnabled.ShouldBeTrue();
        registeredUser.LockoutEnabled.ShouldBeFalse();
        registeredUser.StreamerId.ShouldBe(87);
        var identity = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        var invitation = await identity.Invitations.SingleAsync(
            item => item.UserId == user.Id, TestContext.Current.CancellationToken);
        invitation.IsRegistered.ShouldBeTrue();
        invitation.RegistrationDate.ShouldNotBeNull();
        (await userManager.CheckPasswordAsync(registeredUser, UserTestData.ValidPassword)).ShouldBeTrue();
    }

    [Fact]
    public async Task Invalid_registration_context_is_forbidden()
    {
        var response = await App.CreateAnonymousClient().PostAsJsonAsync(
            "/api/users/register",
            new
            {
                context = "invalid",
                displayName = "New User",
                username = "new-user",
                password = UserTestData.ValidPassword,
                confirmPassword = UserTestData.ValidPassword
            },
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
