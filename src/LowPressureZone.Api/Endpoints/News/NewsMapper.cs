using FastEndpoints;

namespace LowPressureZone.Api.Endpoints.News;

[RegisterService<NewsMapper>(LifeTime.Singleton)]
public sealed class NewsMapper : IResponseMapper
{
    public NewsResponse FromEntity(Domain.NewsAggregate.News entity) => new()
    {
        Id = entity.Id,
        Title = entity.Title,
        Content = entity.Content,
        PublishedAt = entity.PublishedAt
    };
}