using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.News;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.News;

public sealed class GetNewsByIdTests(ApiFixture app) : ApiTestBase(app)
{
    private static string Route(Guid id) => $"/api/news/{id}";

    [Fact]
    public async Task Anonymous_user_can_read_a_news_item()
    {
        var news = await App.AddNewsAsync("Title", "Content");
        await App.AddNewsAsync("Other");

        var response = await App.CreateAnonymousClient().GetAsync(Route(news.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<NewsResponse>(TestContext.Current.CancellationToken);
        body.ShouldNotBeNull();
        body.Id.ShouldBe(news.Id);
        body.Title.ShouldBe("Title");
        body.Content.ShouldBe("Content");
        body.PublishedAt.ShouldBe(news.PublishedAt, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public async Task Unknown_news_is_not_found()
    {
        var response = await App.CreateAnonymousClient()
                                .GetAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("/api/news/not-a-guid")]
    [InlineData("/api/news/00000000-0000-0000-0000-000000000000/extra")]
    [InlineData("/api/news-items")]
    public async Task Unmatched_news_route_is_not_found(string path)
    {
        var response = await App.CreateAnonymousClient().GetAsync(path, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Unsupported_method_on_a_news_route_is_not_served_the_client()
    {
        var news = await App.AddNewsAsync("Title");

        var response = await App.CreateAnonymousClient()
                                .PatchAsync(Route(news.Id), null, TestContext.Current.CancellationToken);

        response.IsSuccessStatusCode.ShouldBeFalse();
    }
}
