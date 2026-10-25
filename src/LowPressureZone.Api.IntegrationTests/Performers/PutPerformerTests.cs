using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.Performers;
using LowPressureZone.Api.IntegrationTests.Infrastructure;
using LowPressureZone.Identity.Entities;

namespace LowPressureZone.Api.IntegrationTests.Performers;

public sealed class PutPerformerTests(ApiFixture app) : ApiTestBase(app)
{
    private static string Route(Guid id) => $"/api/performers/{id}";

    private static PerformerRequest Request(string name = "Edited", string? socialUrl = "https://edited.example.com")
        => new() { Name = name, SocialUrl = socialUrl };

    private async Task<HttpResponseMessage> PutAsync(AppUser user, Guid id, PerformerRequest request)
        => await App.CreateClientFor(user)
                    .PutAsJsonAsync(Route(id), request, TestContext.Current.CancellationToken);

    [Fact]
    public async Task Creator_updates_performer()
    {
        var creator = await App.CreateUserAsync("creator");
        var performer = await App.AddPerformerAsync(creator.Id, "Original", "https://original.example.com");

        var response = await PutAsync(creator, performer.Id, Request("  Edited  ", null));

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var updated = (await App.GetPerformersAsync()).ShouldHaveSingleItem();
        updated.Name.ShouldBe("Edited");
        updated.SocialUrl.ShouldBe(string.Empty);
    }

    [Fact]
    public async Task Admin_updates_performer()
    {
        var creator = await App.CreateUserAsync("creator");
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var performer = await App.AddPerformerAsync(creator.Id, "Original");

        var response = await PutAsync(admin, performer.Id, Request());

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await App.GetPerformersAsync()).ShouldHaveSingleItem().Name.ShouldBe("Edited");
    }

    [Fact]
    public async Task Other_authenticated_user_is_unauthorized()
    {
        var creator = await App.CreateUserAsync("creator");
        var other = await App.CreateUserAsync("other");
        var performer = await App.AddPerformerAsync(creator.Id, "Original");

        var response = await PutAsync(other, performer.Id, Request());

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await App.GetPerformersAsync()).ShouldHaveSingleItem().Name.ShouldBe("Original");
    }

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var creator = await App.CreateUserAsync("creator");
        var performer = await App.AddPerformerAsync(creator.Id, "Original");

        var response = await App.CreateAnonymousClient()
                                .PutAsJsonAsync(Route(performer.Id), Request(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Unknown_performer_is_not_found()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);

        var response = await PutAsync(admin, Guid.NewGuid(), Request());

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Deleted_performer_is_not_found()
    {
        var creator = await App.CreateUserAsync("creator");
        var performer = await App.AddPerformerAsync(creator.Id, "Deleted", isDeleted: true);

        var response = await PutAsync(creator, performer.Id, Request());

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("", "https://edited.example.com", "Required")]
    [InlineData("Edited", "not a URL", "Invalid URL")]
    public async Task Invalid_request_is_a_bad_request_and_changes_nothing(
        string name,
        string? socialUrl,
        string expectedError)
    {
        var creator = await App.CreateUserAsync("creator");
        await App.AddPerformerAsync(creator.Id, "Original", "https://original.example.com");

        var response = await PutAsync(creator, (await App.GetPerformersAsync()).Single().Id, Request(name, socialUrl));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).ShouldContain(expectedError);
        var unchanged = (await App.GetPerformersAsync()).ShouldHaveSingleItem();
        unchanged.Name.ShouldBe("Original");
        unchanged.SocialUrl.ShouldBe("https://original.example.com");
    }

    [Fact]
    public async Task Social_url_longer_than_512_characters_is_a_bad_request_and_changes_nothing()
    {
        var creator = await App.CreateUserAsync("creator");
        var performer = await App.AddPerformerAsync(creator.Id, "Original", "https://original.example.com");

        var response = await PutAsync(creator, performer.Id, Request(socialUrl: "https://a.com/" + new string('a', 499)));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))
            .ShouldContain("Maximum 512 characters");
        var unchanged = (await App.GetPerformersAsync()).ShouldHaveSingleItem();
        unchanged.Name.ShouldBe("Original");
        unchanged.SocialUrl.ShouldBe("https://original.example.com");
    }
}
