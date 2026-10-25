using System.Net;
using LowPressureZone.Api.IntegrationTests.Infrastructure;

namespace LowPressureZone.Api.IntegrationTests.News;

public sealed class DeleteNewsTests(ApiFixture app) : ApiTestBase(app)
{
    private static string Route(Guid id) => $"/api/news/{id}";

    private async Task<HttpResponseMessage> DeleteAsAdminAsync(Guid id)
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        return await App.CreateClientFor(admin).DeleteAsync(Route(id), TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        var news = await App.AddNewsAsync("Title");

        var response = await App.CreateAnonymousClient().DeleteAsync(Route(news.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await App.GetNewsAsync()).ShouldHaveSingleItem();
    }

    [Fact]
    public async Task Organizer_is_forbidden()
    {
        var news = await App.AddNewsAsync("Title");
        var organizer = await App.CreateUserAsync("organizer");
        await App.MakeOrganizerAsync(organizer);

        var response = await App.CreateClientFor(organizer)
                                .DeleteAsync(Route(news.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await App.GetNewsAsync()).ShouldHaveSingleItem();
    }

    [Fact]
    public async Task Admin_deletes_only_the_requested_news()
    {
        var news = await App.AddNewsAsync("Delete me");
        var other = await App.AddNewsAsync("Keep me");

        var response = await DeleteAsAdminAsync(news.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await App.GetNewsAsync()).ShouldHaveSingleItem().Id.ShouldBe(other.Id);
    }

    [Fact]
    public async Task Deleted_news_is_no_longer_readable()
    {
        var news = await App.AddNewsAsync("Title");
        await DeleteAsAdminAsync(news.Id);

        var response = await App.CreateAnonymousClient().GetAsync(Route(news.Id), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Unknown_news_is_not_found()
    {
        var response = await DeleteAsAdminAsync(Guid.NewGuid());

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
