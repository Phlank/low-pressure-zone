namespace LowPressureZone.Api.Endpoints.Page;

public partial class GetPage
{
    public class Response
    {
        public string Title { get; set; } = string.Empty;
        public List<Post> Posts { get; set; } = new();

        public class Post
        {
            public string Content { get; set; } = string.Empty;
            public DateTime PostedDate { get; set; }
        }
    }
}