using FastEndpoints;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.News;

public class GetNewsById(DataContext dataContext) : EndpointWithoutRequest<NewsResponse, NewsMapper>
{
    public override void Configure()
    {
        Get("news/{id:guid}");
        AllowAnonymous();
        Description(builder => builder.WithTags("News")
                                      .Produces<NewsResponse>());
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var newsItem = await dataContext.News
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(news => news.Id == id, ct);
        if (newsItem == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var response = Map.FromEntity(newsItem);
        await SendOkAsync(response, ct);
    }
}