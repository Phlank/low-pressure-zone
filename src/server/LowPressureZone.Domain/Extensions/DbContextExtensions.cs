using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Domain.Extensions;

public static class DbContextExtensions
{
    public static bool Has<TEntity>(this DbContext context, Guid id) where TEntity : BaseEntity
        => context.Set<TEntity>().Any(e => e.Id == id);

    public static Task<bool> HasAsync<TEntity>(this DbContext context, Guid id, CancellationToken ct = default)
        where TEntity : BaseEntity
        => context.Set<TEntity>().AnyAsync(e => e.Id == id, ct);
}