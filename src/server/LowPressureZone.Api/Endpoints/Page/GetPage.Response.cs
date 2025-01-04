namespace LowPressureZone.Api.Endpoints.Page;

public sealed class GetPageResponse
{
    public string Title { get; set; } = string.Empty;
    public List<Post> Posts { get; set; } = new();

    public class Post
    {
        public string Content { get; set; } = string.Empty;
        public DateTime PostedDate { get; set; }
    }
}