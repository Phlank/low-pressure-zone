using FastEndpoints;

namespace LowPressureZone.Api.Endpoints.News;

public sealed class NewsMapper : IRequestMapper, IResponseMapper
{
    public Domain.Entities.News ToEntity(NewsRequest request) =>
        new()
        {
            Title = request.Title,
            Body = request.Body
        };

    public NewsResponse FromEntity(Domain.Entities.News entity) => new()
    {
        Id = entity.Id,
        Title = entity.Title,
        Body = entity.Body,
        CreatedAt = entity.CreatedDate
    };
}