using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.News;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.News;

public sealed class PostNewsTests(ApiFixture app) : ApiTestBase(app)
{
    private const string Route = "/api/news";

    private static NewsRequest Request(string title = "Title", string content = "Content")
        => new() { Title = title, Content = content };

    private async Task<HttpResponseMessage> PostAsAdminAsync(NewsRequest request)
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        return await App.CreateClientFor(admin).PostAsJsonAsync(Route, request, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var response = await App.CreateAnonymousClient()
                                .PostAsJsonAsync(Route, Request(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await App.GetNewsAsync()).ShouldBeEmpty();
    }

    [Fact]
    public async Task Organizer_is_forbidden()
    {
        var organizer = await App.CreateUserAsync("organizer");
        await App.MakeOrganizerAsync(organizer);

        var response = await App.CreateClientFor(organizer)
                                .PostAsJsonAsync(Route, Request(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await App.GetNewsAsync()).ShouldBeEmpty();
    }

    [Fact]
    public async Task Admin_creates_news()
    {
        var before = DateTimeOffset.UtcNow;

        var response = await PostAsAdminAsync(Request("  Title  ", "  Content  "));

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var news = (await App.GetNewsAsync()).ShouldHaveSingleItem();
        news.Title.ShouldBe("Title");
        news.Content.ShouldBe("Content");
        news.PublishedAt.ShouldBeInRange(before.AddSeconds(-1), DateTimeOffset.UtcNow.AddSeconds(1));
        response.Headers.Location.ShouldNotBeNull();
        response.Headers.Location.ToString().ShouldEndWith($"/news/{news.Id}");
        response.Headers.GetValues("Access-Control-Expose-Headers").ShouldContain("location");
    }

    [Fact]
    public async Task Created_location_resolves_to_the_new_news_item()
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        var client = App.CreateClientFor(admin);
        var created = await client.PostAsJsonAsync(Route, Request(), TestContext.Current.CancellationToken);

        var response = await client.GetAsync(created.Headers.Location, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<NewsResponse>(TestContext.Current.CancellationToken);
        body.ShouldNotBeNull();
        body.Title.ShouldBe("Title");
    }

    [Theory]
    [InlineData("", "Content", "title")]
    [InlineData("   ", "Content", "title")]
    [InlineData("Title", "", "content")]
    [InlineData("Title", "   ", "content")]
    public async Task Missing_field_is_a_bad_request(string title, string content, string invalidField)
    {
        var response = await PostAsAdminAsync(Request(title, content));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        body.ShouldContain($"\"{invalidField}\"", Case.Insensitive);
        body.ShouldContain("Required");
        (await App.GetNewsAsync()).ShouldBeEmpty();
    }

    [Fact]
    public async Task Title_longer_than_256_characters_is_a_bad_request()
    {
        var response = await PostAsAdminAsync(Request(new string('a', 257)));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))
            .ShouldContain("Cannot be more than 256 characters");
        (await App.GetNewsAsync()).ShouldBeEmpty();
    }

    [Fact]
    public async Task Content_longer_than_16384_characters_is_a_bad_request()
    {
        var response = await PostAsAdminAsync(Request(content: new string('a', 16385)));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))
            .ShouldContain("Cannot be more than 16384 characters");
        (await App.GetNewsAsync()).ShouldBeEmpty();
    }

    [Fact]
    public async Task Maximum_length_title_and_content_are_accepted()
    {
        var response = await PostAsAdminAsync(Request(new string('t', 256), new string('c', 16384)));

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var news = (await App.GetNewsAsync()).ShouldHaveSingleItem();
        news.Title.Length.ShouldBe(256);
        news.Content.Length.ShouldBe(16384);
    }
}
