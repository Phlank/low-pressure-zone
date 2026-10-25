using System.Net;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Performers;

public sealed class DeletePerformerTests(ApiFixture app) : ApiTestBase(app)
{
    private static string Route(Guid id) => $"/api/performers/{id}";

    [Fact]
    public async Task Creator_soft_deletes_performer()
    {
        var creator = await App.CreateUserAsync("creator");
        var performer = await App.AddPerformerAsync(creator.Id, "Performer");

        var response = await App.CreateClientFor(creator)
                                .DeleteAsync(Route(performer.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await App.GetPerformersAsync()).ShouldHaveSingleItem().IsDeleted.ShouldBeTrue();
        (await App.CreateAnonymousClient().GetAsync(Route(performer.Id), TestContext.Current.CancellationToken))
            .StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Admin_can_delete_another_users_performer()
    {
        var creator = await App.CreateUserAsync("creator");
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var performer = await App.AddPerformerAsync(creator.Id, "Performer");

        var response = await App.CreateClientFor(admin)
                                .DeleteAsync(Route(performer.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await App.GetPerformersAsync()).ShouldHaveSingleItem().IsDeleted.ShouldBeTrue();
    }

    [Fact]
    public async Task Other_authenticated_user_is_unauthorized()
    {
        var creator = await App.CreateUserAsync("creator");
        var other = await App.CreateUserAsync("other");
        var performer = await App.AddPerformerAsync(creator.Id, "Performer");

        var response = await App.CreateClientFor(other)
                                .DeleteAsync(Route(performer.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await App.GetPerformersAsync()).ShouldHaveSingleItem().IsDeleted.ShouldBeFalse();
    }

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var creator = await App.CreateUserAsync("creator");
        var performer = await App.AddPerformerAsync(creator.Id, "Performer");

        var response = await App.CreateAnonymousClient()
                                .DeleteAsync(Route(performer.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Unknown_performer_is_not_found()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);

        var response = await App.CreateClientFor(admin)
                                .DeleteAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Already_deleted_performer_is_not_found()
    {
        var creator = await App.CreateUserAsync("creator");
        var performer = await App.AddPerformerAsync(creator.Id, "Deleted", isDeleted: true);

        var response = await App.CreateClientFor(creator)
                                .DeleteAsync(Route(performer.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
