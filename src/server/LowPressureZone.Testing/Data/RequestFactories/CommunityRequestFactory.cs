using LowPressureZone.Api.Endpoints.Communities;

namespace LowPressureZone.Testing.Data.RequestFactories;

public static class CommunityRequestFactory
{
    public static CommunityRequest Create(string? name = null, string? url = null) => new()
    {
        Name = name ?? "Valid Name",
        Url = url ?? "https://validurl.com"
    };
}