using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.Performers;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Performers;

public sealed class GetPerformerByIdTests(ApiFixture app) : ApiTestBase(app)
{
    private static string Route(Guid id) => $"/api/performers/{id}";

    [Fact]
    public async Task Anonymous_user_can_read_performer()
    {
        var creator = await App.CreateUserAsync("creator");
        var performer = await App.AddPerformerAsync(creator.Id, "Name", "https://example.com");

        var response = await App.CreateAnonymousClient()
                                .GetAsync(Route(performer.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PerformerResponse>(TestContext.Current.CancellationToken);
        body.ShouldNotBeNull();
        body.Id.ShouldBe(performer.Id);
        body.Name.ShouldBe("Name");
        body.SocialUrl.ShouldBe("https://example.com");
        body.IsEditable.ShouldBeFalse();
        body.IsDeletable.ShouldBeFalse();
        body.IsLinkableToTimeslot.ShouldBeFalse();
    }

    [Fact]
    public async Task Creator_has_management_permissions()
    {
        var creator = await App.CreateUserAsync("creator");
        var performer = await App.AddPerformerAsync(creator.Id, "Name");

        var response = await App.CreateClientFor(creator)
                                .GetAsync(Route(performer.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PerformerResponse>(TestContext.Current.CancellationToken);
        body.ShouldNotBeNull();
        body.IsEditable.ShouldBeTrue();
        body.IsDeletable.ShouldBeTrue();
        body.IsLinkableToTimeslot.ShouldBeTrue();
    }

    [Fact]
    public async Task Unknown_performer_is_not_found()
    {
        var response = await App.CreateAnonymousClient()
                                .GetAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Deleted_performer_is_not_found()
    {
        var creator = await App.CreateUserAsync("creator");
        var performer = await App.AddPerformerAsync(creator.Id, "Deleted", isDeleted: true);

        var response = await App.CreateAnonymousClient()
                                .GetAsync(Route(performer.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
