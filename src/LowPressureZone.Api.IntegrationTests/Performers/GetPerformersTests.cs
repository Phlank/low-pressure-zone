using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.Performers;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Performers;

public sealed class GetPerformersTests(ApiFixture app) : ApiTestBase(app)
{
    private const string Route = "/api/performers";

    private static async Task<List<PerformerResponse>> ReadAsync(HttpResponseMessage response)
    {
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<List<PerformerResponse>>(
                    TestContext.Current.CancellationToken))!;
    }

    [Fact]
    public async Task Anonymous_user_can_list_performers()
    {
        var creator = await App.CreateUserAsync("creator");
        await App.AddPerformerAsync(creator.Id, "Performer");

        var performer = (await ReadAsync(await App.CreateAnonymousClient()
                                                 .GetAsync(Route, TestContext.Current.CancellationToken)))
            .ShouldHaveSingleItem();

        performer.Name.ShouldBe("Performer");
        performer.IsEditable.ShouldBeFalse();
        performer.IsDeletable.ShouldBeFalse();
        performer.IsLinkableToTimeslot.ShouldBeFalse();
    }

    [Fact]
    public async Task Returns_performers_alphabetically_and_hides_deleted_performers()
    {
        var creator = await App.CreateUserAsync("creator");
        await App.AddPerformerAsync(creator.Id, "Zed");
        await App.AddPerformerAsync(creator.Id, "Deleted", isDeleted: true);
        await App.AddPerformerAsync(creator.Id, "Amy");

        var performers = await ReadAsync(await App.CreateAnonymousClient()
                                                  .GetAsync(Route, TestContext.Current.CancellationToken));

        performers.Select(performer => performer.Name).ShouldBe(["Amy", "Zed"]);
    }

    [Fact]
    public async Task Creator_can_edit_delete_and_link_own_performer()
    {
        var creator = await App.CreateUserAsync("creator");
        await App.AddPerformerAsync(creator.Id, "Performer");

        var performer = (await ReadAsync(await App.CreateClientFor(creator)
                                                 .GetAsync(Route, TestContext.Current.CancellationToken)))
            .ShouldHaveSingleItem();

        performer.IsEditable.ShouldBeTrue();
        performer.IsDeletable.ShouldBeTrue();
        performer.IsLinkableToTimeslot.ShouldBeTrue();
    }

    [Fact]
    public async Task Admin_can_edit_delete_and_link_every_performer()
    {
        var creator = await App.CreateUserAsync("creator");
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        await App.AddPerformerAsync(creator.Id, "Performer");

        var performer = (await ReadAsync(await App.CreateClientFor(admin)
                                                 .GetAsync(Route, TestContext.Current.CancellationToken)))
            .ShouldHaveSingleItem();

        performer.IsEditable.ShouldBeTrue();
        performer.IsDeletable.ShouldBeTrue();
        performer.IsLinkableToTimeslot.ShouldBeTrue();
    }

    [Fact]
    public async Task Other_authenticated_user_cannot_edit_delete_or_link_performer()
    {
        var creator = await App.CreateUserAsync("creator");
        var other = await App.CreateUserAsync("other");
        await App.AddPerformerAsync(creator.Id, "Performer");

        var performer = (await ReadAsync(await App.CreateClientFor(other)
                                                 .GetAsync(Route, TestContext.Current.CancellationToken)))
            .ShouldHaveSingleItem();

        performer.IsEditable.ShouldBeFalse();
        performer.IsDeletable.ShouldBeFalse();
        performer.IsLinkableToTimeslot.ShouldBeFalse();
    }
}
