using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.Performers;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Performers;

public sealed class PostPerformerTests(ApiFixture app) : ApiTestBase(app)
{
    private const string Route = "/api/performers";

    private static PerformerRequest Request(string name = "Performer", string? socialUrl = null)
        => new() { Name = name, SocialUrl = socialUrl };

    [Fact]
    public async Task Authenticated_user_creates_performer_and_is_assigned_permissions()
    {
        var creator = await App.CreateUserAsync("creator");

        var response = await App.CreateClientFor(creator)
                                .PostAsJsonAsync(Route, Request("  Name  ", "https://example.com"),
                                                 TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var performer = (await App.GetPerformersAsync()).ShouldHaveSingleItem();
        performer.CreatorUserId.ShouldBe(creator.Id);
        performer.Name.ShouldBe("Name");
        performer.SocialUrl.ShouldBe("https://example.com");
        performer.IsDeleted.ShouldBeFalse();
        response.Headers.Location.ShouldNotBeNull();
        response.Headers.Location.ToString().ShouldEndWith($"/performers/{performer.Id}");
        response.Headers.GetValues("Access-Control-Expose-Headers").ShouldContain("location");

        var readResponse = await App.CreateClientFor(creator)
                                    .GetAsync(response.Headers.Location, TestContext.Current.CancellationToken);
        var body = await readResponse.Content.ReadFromJsonAsync<PerformerResponse>(TestContext.Current.CancellationToken);
        body.ShouldNotBeNull();
        body.IsEditable.ShouldBeTrue();
        body.IsDeletable.ShouldBeTrue();
        body.IsLinkableToTimeslot.ShouldBeTrue();
    }

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var response = await App.CreateAnonymousClient()
                                .PostAsJsonAsync(Route, Request(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await App.GetPerformersAsync()).ShouldBeEmpty();
    }

    [Theory]
    [InlineData("", null, "Required")]
    [InlineData("   ", null, "Required")]
    [InlineData("Performer", "not a URL", "Invalid URL")]
    public async Task Invalid_request_is_a_bad_request(string name, string? socialUrl, string expectedError)
    {
        var creator = await App.CreateUserAsync("creator");

        var response = await App.CreateClientFor(creator)
                                .PostAsJsonAsync(Route, Request(name, socialUrl), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).ShouldContain(expectedError);
        (await App.GetPerformersAsync()).ShouldBeEmpty();
    }

    [Fact]
    public async Task Name_longer_than_64_characters_is_a_bad_request()
    {
        var creator = await App.CreateUserAsync("creator");

        var response = await App.CreateClientFor(creator)
                                .PostAsJsonAsync(Route, Request(new string('a', 65)),
                                                 TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))
            .ShouldContain("Maximum 64 characters");
        (await App.GetPerformersAsync()).ShouldBeEmpty();
    }

    [Fact]
    public async Task Maximum_length_social_url_is_accepted()
    {
        var creator = await App.CreateUserAsync("creator");
        var socialUrl = "https://a.com/" + new string('a', 498);
        socialUrl.Length.ShouldBe(512);

        var response = await App.CreateClientFor(creator)
                                .PostAsJsonAsync(Route, Request(socialUrl: socialUrl),
                                                 TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        (await App.GetPerformersAsync()).ShouldHaveSingleItem().SocialUrl.ShouldBe(socialUrl);
    }

    [Fact]
    public async Task Social_url_longer_than_512_characters_is_a_bad_request()
    {
        var creator = await App.CreateUserAsync("creator");
        var socialUrl = "https://a.com/" + new string('a', 499);

        var response = await App.CreateClientFor(creator)
                                .PostAsJsonAsync(Route, Request(socialUrl: socialUrl),
                                                 TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))
            .ShouldContain("Maximum 512 characters");
        (await App.GetPerformersAsync()).ShouldBeEmpty();
    }
}
