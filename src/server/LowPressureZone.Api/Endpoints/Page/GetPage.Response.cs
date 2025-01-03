using FastEndpoints;
using static LowPressureZone.Api.Endpoints.Page.GetPage;

namespace LowPressureZone.Api.Endpoints.Page;

public partial class GetPage : Endpoint<Request>
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