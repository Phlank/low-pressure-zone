namespace LowPressureZone.Api.Endpoints.News;

public sealed class NewsRequest
{
    public required string Title { get; set; }
    public required string Body { get; set; }
}