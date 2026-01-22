using LowPressureZone.Domain.Entities;

namespace LowPressureZone.Testing.Data.EntityFactories;

public static class CommunityFactory
{
    public static Community Create(
        IEnumerable<CommunityRelationship>? relationships = null,
        bool nullRelationships = false,
        Guid? id = null,
        string? name = null,
        string? url = null,
        bool isDeleted = false) => new()
    {
        Id = id ?? Guid.Empty,
        Name = name ?? "Test Community",
        Url = url ?? "https://testcommunity.com",
        Relationships = nullRelationships ? null! : relationships?.ToList() ?? [],
        IsDeleted = isDeleted
    };
}