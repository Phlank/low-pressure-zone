using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Domain.Extensions;

public static class DbContextExtensions
{
    public static bool Has<TEntity>(this DbContext context, Guid id) where TEntity : BaseEntity
    {
        return context.Set<TEntity>().Any(e => e.Id == id);
    }
}
