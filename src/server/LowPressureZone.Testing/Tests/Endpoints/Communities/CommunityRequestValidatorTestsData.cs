using LowPressureZone.Domain.Entities;
using LowPressureZone.Testing.Data.EntityFactories;

namespace LowPressureZone.Testing.Tests.Endpoints.Communities;

public class CommunityRequestValidatorTestsData
{
    public static readonly Guid CommunityId = Guid.NewGuid();
    public const string CommunityName = "Test Community 1";
    public const string CommunityUrl = "https://testcommunity1.com";
    
    public static readonly IEnumerable<Community> Communities =
    [
        CommunityFactory.Create(id: CommunityId, name: CommunityName, url: CommunityUrl)
    ];
}