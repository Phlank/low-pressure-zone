using FastEndpoints;
using static LowPressureZone.Api.Endpoints.Page.GetPage;

namespace LowPressureZone.Api.Endpoints.Page;

public partial class GetPage : Endpoint<Request>
{
    public class Request
    {
        public string Name { get; set; } = string.Empty;
    }
}
