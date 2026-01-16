using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Endpoints.News;

public class PostNews(DataContext dataContext) : EndpointWithMapper<NewsRequest, NewsMapper>
{
    public override void Configure()
    {
        Post("/news");
        Roles(RoleNames.Admin);
        Description(builder => builder.WithTags("News")
                                      .Produces(201));
    }

    public override async Task HandleAsync(NewsRequest req, CancellationToken ct)
    {
        var newsItem = Map.ToEntity(req);
        await dataContext.News.AddAsync(newsItem, ct);
        await dataContext.SaveChangesAsync(ct);
        HttpContext.ExposeLocation();
        await SendCreatedAtAsync<GetNewsById>(new
        {
            newsItem.Id
        }, Response, cancellation: ct);
    }
}