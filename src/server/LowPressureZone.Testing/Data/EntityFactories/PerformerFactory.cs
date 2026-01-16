using LowPressureZone.Domain.Entities;

namespace LowPressureZone.Testing.Data.EntityFactories;

public static class PerformerFactory
{
    public static Performer Create(
        Guid? id = null,
        Guid? userId = null,
        string? name = null,
        string? url = null,
        bool isDeleted = false) =>
        new()
        {
            Id = id ?? Guid.Empty,
            Name = name ?? "Test Performer",
            Url = url ?? "https://testperformer.com",
            LinkedUserIds = userId is not null ? [userId.Value] : [],
            IsDeleted = isDeleted
        };
}