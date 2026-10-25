using System.Net;
using System.Net.Http.Json;
using LowPressureZone.Api.Endpoints.News;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.News;

public sealed class GetNewsTests(ApiFixture app) : ApiTestBase(app)
{
    private const string Route = "/api/news";

    private static async Task<List<NewsResponse>> ReadAsync(HttpResponseMessage response)
    {
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<List<NewsResponse>>(TestContext.Current.CancellationToken))!;
    }

    [Fact]
    public async Task Anonymous_user_can_read_news()
    {
        var news = await App.AddNewsAsync("Title", "Content");

        var response = (await ReadAsync(await App.CreateAnonymousClient()
                                                 .GetAsync(Route, TestContext.Current.CancellationToken)))
            .ShouldHaveSingleItem();

        response.Id.ShouldBe(news.Id);
        response.Title.ShouldBe("Title");
        response.Content.ShouldBe("Content");
        response.PublishedAt.ShouldBe(news.PublishedAt, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public async Task Returns_newest_news_first()
    {
        var now = DateTimeOffset.UtcNow;
        await App.AddNewsAsync("Middle", publishedAt: now.AddDays(-1));
        await App.AddNewsAsync("Newest", publishedAt: now);
        await App.AddNewsAsync("Oldest", publishedAt: now.AddDays(-2));

        var news = await ReadAsync(await App.CreateAnonymousClient()
                                            .GetAsync(Route, TestContext.Current.CancellationToken));

        news.Select(n => n.Title).ShouldBe(["Newest", "Middle", "Oldest"]);
    }

    [Fact]
    public async Task No_news_returns_an_empty_list()
    {
        var news = await ReadAsync(await App.CreateAnonymousClient()
                                            .GetAsync(Route, TestContext.Current.CancellationToken));

        news.ShouldBeEmpty();
    }
}
