using FastEndpoints;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.News;

public class GetNews(DataContext dataContext) : EndpointWithoutRequest<IEnumerable<NewsResponse>, NewsMapper>
{
    public override void Configure()
    {
        Get("/news");
        AllowAnonymous();
        Description(builder => builder.WithTags("News")
                                      .Produces<IEnumerable<NewsResponse>>());
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var newsItems = await dataContext.News
                                         .AsNoTracking()
                                         .OrderByDescending(news => news.CreatedDate)
                                         .ToListAsync(ct);
        var responses = newsItems.Select(Map.FromEntity);
        await SendOkAsync(responses, ct);
    }
}