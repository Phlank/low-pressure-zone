using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.News;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.News;

public sealed class PutNewsTests(ApiFixture app) : ApiTestBase(app)
{
    private static readonly DateTimeOffset OriginalPublishedAt = new(2026, 1, 15, 12, 0, 0, TimeSpan.Zero);

    private static string Route(Guid id) => $"/api/news/{id}";

    private static NewsRequest Request(string title = "Edited title", string content = "Edited content")
        => new() { Title = title, Content = content };

    private async Task<Guid> AddOriginalAsync()
        => (await App.AddNewsAsync("Original title", "Original content", OriginalPublishedAt)).Id;

    private async Task<HttpResponseMessage> PutAsAdminAsync(Guid id, NewsRequest request)
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        return await App.CreateClientFor(admin).PutAsJsonAsync(Route(id), request, TestContext.Current.CancellationToken);
    }

    private async Task ShouldBeUnchangedAsync()
    {
        var news = (await App.GetNewsAsync()).ShouldHaveSingleItem();
        news.Title.ShouldBe("Original title");
        news.Content.ShouldBe("Original content");
    }

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var id = await AddOriginalAsync();

        var response = await App.CreateAnonymousClient()
                                .PutAsJsonAsync(Route(id), Request(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        await ShouldBeUnchangedAsync();
    }

    [Fact]
    public async Task Organizer_is_forbidden()
    {
        var id = await AddOriginalAsync();
        var organizer = await App.CreateUserAsync("organizer");
        await App.MakeOrganizerAsync(organizer);

        var response = await App.CreateClientFor(organizer)
                                .PutAsJsonAsync(Route(id), Request(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        await ShouldBeUnchangedAsync();
    }

    [Fact]
    public async Task Admin_edits_news_without_changing_its_publish_time()
    {
        var id = await AddOriginalAsync();

        var response = await PutAsAdminAsync(id, Request("  Edited title  ", "  Edited content  "));

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var news = (await App.GetNewsAsync()).ShouldHaveSingleItem();
        news.Id.ShouldBe(id);
        news.Title.ShouldBe("Edited title");
        news.Content.ShouldBe("Edited content");
        news.PublishedAt.ShouldBe(OriginalPublishedAt);
    }

    [Fact]
    public async Task Unknown_news_is_not_found()
    {
        var response = await PutAsAdminAsync(Guid.NewGuid(), Request());

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("", "Edited content", "Required")]
    [InlineData("Edited title", "   ", "Required")]
    [InlineData("Edited title", null, "Cannot be more than 16384 characters")]
    [InlineData(null, "Edited content", "Cannot be more than 256 characters")]
    public async Task Invalid_request_is_a_bad_request_and_changes_nothing(
        string? title,
        string? content,
        string expectedError)
    {
        var id = await AddOriginalAsync();

        var response = await PutAsAdminAsync(id, Request(title ?? new string('a', 257), content ?? new string('a', 16385)));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).ShouldContain(expectedError);
        await ShouldBeUnchangedAsync();
    }
}
