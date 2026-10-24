namespace LowPressureZone.Api.Endpoints.News;

public sealed class NewsResponse
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required DateTimeOffset PublishedAt { get; set; }
}