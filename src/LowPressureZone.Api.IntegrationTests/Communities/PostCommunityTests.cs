using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.Communities;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.Communities;

public sealed class PostCommunityTests(ApiFixture app) : ApiTestBase(app)
{
    private const string Route = "/api/communities";

    private static CommunityRequest Request(string name = "Alpha", string socialUrl = "https://alpha.example.com")
        => new() { Name = name, SocialUrl = socialUrl };

    private async Task<HttpResponseMessage> PostAsAdminAsync(CommunityRequest request)
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        return await App.CreateClientFor(admin)
                        .PostAsJsonAsync(Route, request, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var response = await App.CreateAnonymousClient()
                                .PostAsJsonAsync(Route, Request(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await App.GetCommunitiesAsync()).ShouldBeEmpty();
    }

    [Fact]
    public async Task Organizer_is_forbidden()
    {
        var organizer = await App.CreateUserAsync("organizer");
        await App.MakeOrganizerAsync(organizer);

        var response = await App.CreateClientFor(organizer)
                                .PostAsJsonAsync(Route, Request(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await App.GetCommunitiesAsync()).ShouldNotContain(c => c.Name == "Alpha");
    }

    [Fact]
    public async Task Admin_creates_community()
    {
        var response = await PostAsAdminAsync(Request("  Alpha  ", "https://alpha.example.com"));

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var community = (await App.GetCommunitiesAsync()).ShouldHaveSingleItem();
        community.Name.ShouldBe("Alpha");
        community.SocialUrl.ShouldBe("https://alpha.example.com");
        community.IsDeleted.ShouldBeFalse();
        community.Relationships.ShouldBeEmpty();
        response.Headers.Location.ShouldNotBeNull();
        response.Headers.Location.ToString().ShouldEndWith($"/communities/{community.Id}");
        response.Headers.GetValues("Access-Control-Expose-Headers").ShouldContain("location");
    }

    [Fact]
    public async Task Created_location_resolves_to_the_new_community()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var client = App.CreateClientFor(admin);
        var created = await client.PostAsJsonAsync(Route, Request(), TestContext.Current.CancellationToken);

        var response = await client.GetAsync(created.Headers.Location, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<CommunityResponse>(TestContext.Current.CancellationToken);
        body.ShouldNotBeNull();
        body.Name.ShouldBe("Alpha");
    }

    [Theory]
    [InlineData("", "https://alpha.example.com", "Required")]
    [InlineData("   ", "https://alpha.example.com", "Required")]
    [InlineData("Alpha", "not a url", "Invalid URL")]
    public async Task Invalid_request_is_a_bad_request(string name, string socialUrl, string expectedError)
    {
        var response = await PostAsAdminAsync(Request(name, socialUrl));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).ShouldContain(expectedError);
        (await App.GetCommunitiesAsync()).ShouldBeEmpty();
    }

    [Fact]
    public async Task Name_longer_than_128_characters_is_a_bad_request()
    {
        var response = await PostAsAdminAsync(Request(new string('a', 129)));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))
            .ShouldContain("Name cannot exceed 128 characters");
        (await App.GetCommunitiesAsync()).ShouldBeEmpty();
    }

    [Fact]
    public async Task Social_url_longer_than_512_characters_is_a_bad_request()
    {
        var response = await PostAsAdminAsync(Request(socialUrl: "https://example.com/" + new string('a', 493)));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))
            .ShouldContain("Cannot exceed 512 characters");
        (await App.GetCommunitiesAsync()).ShouldBeEmpty();
    }

    [Fact]
    public async Task Duplicate_name_is_a_bad_request()
    {
        await App.AddCommunityAsync("Alpha", "https://existing.example.com");

        var response = await PostAsAdminAsync(Request("Alpha", "https://new.example.com"));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).ShouldContain("Name is already in use");
        (await App.GetCommunitiesAsync()).ShouldHaveSingleItem();
    }

    [Fact]
    public async Task Name_of_a_deleted_community_is_still_reserved()
    {
        await App.AddCommunityAsync("Alpha", "https://existing.example.com", c => c.Delete());

        var response = await PostAsAdminAsync(Request("Alpha", "https://new.example.com"));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))
            .ShouldContain("Name is already in use");
    }

    [Fact]
    public async Task Duplicate_social_url_is_a_bad_request()
    {
        await App.AddCommunityAsync("Existing", "https://alpha.example.com");

        var response = await PostAsAdminAsync(Request("Alpha", "https://alpha.example.com"));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).ShouldContain("Social URL is already in use");
        (await App.GetCommunitiesAsync()).ShouldHaveSingleItem();
    }
}
