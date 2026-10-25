using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.Users;
using LowPressureZone.Api.IntegrationTests.Infrastructure;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.IntegrationTests.Users.Enabled;

public sealed class PutEnabledTests(ApiFixture app) : ApiTestBase(app)
{
    [Fact]
    public async Task Admin_can_disable_and_reenable_user()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var target = await App.CreateUserAsync("target");
        var client = App.CreateClientFor(admin);

        var disable = await client.PutAsJsonAsync($"/api/users/{target.Id}/enabled",
                                                  new { enabled = false },
                                                  TestContext.Current.CancellationToken);
        disable.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        await using (var scope = App.Services.CreateAsyncScope())
        {
            var user = await scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>()
                              .FindByIdAsync(target.Id.ToString());
            user.ShouldNotBeNull();
            user.LockoutEnabled.ShouldBeTrue();
        }

        var enable = await client.PutAsJsonAsync($"/api/users/{target.Id}/enabled",
                                                 new { enabled = true },
                                                 TestContext.Current.CancellationToken);
        enable.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        await using var finalScope = App.Services.CreateAsyncScope();
        var updated = await finalScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>()
                                  .FindByIdAsync(target.Id.ToString());
        updated.ShouldNotBeNull();
        updated.LockoutEnabled.ShouldBeFalse();
    }

    [Fact]
    public async Task Missing_user_is_not_found()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);

        var response = await App.CreateClientFor(admin).PutAsJsonAsync(
            $"/api/users/{Guid.NewGuid()}/enabled", new { enabled = false }, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Non_admin_cannot_change_enabled_status()
    {
        var user = await App.CreateUserAsync("user");
        var target = await App.CreateUserAsync("target");

        var response = await App.CreateClientFor(user).PutAsJsonAsync(
            $"/api/users/{target.Id}/enabled", new { enabled = false }, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
