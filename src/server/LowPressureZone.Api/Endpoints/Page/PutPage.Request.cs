namespace LowPressureZone.Api.Endpoints.Page;

public partial class PutPage
{
    public class Request
    {
        public string Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
