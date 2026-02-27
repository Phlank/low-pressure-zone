using LowPressureZone.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Domain.Extensions;

public static class DbContextOptionsBuilderExtensions
{
    private static Community SeedCommunity => new()
    {
        Name = "Default Community",
        Url = "https://lowpressurezone.com",
        IsDeleted = false
    };

    private static void Seed_AddCommunity(DbContext context)
    {
        var communities = context.Set<Community>();
        if (!communities.Any())
        {
            communities.Add(SeedCommunity);
            context.SaveChanges();
        }
    }
    
    private static async Task Seed_AddCommunityAsync(DbContext context, CancellationToken ct)
    {
        var communities = context.Set<Community>();
        if (!await communities.AnyAsync(ct))
        {
            communities.Add(SeedCommunity);
            await context.SaveChangesAsync(ct);
        }
    }

    extension(DbContextOptionsBuilder optionsBuilder)
    {
        public void ConfigureDomainSeeding() =>
            optionsBuilder.UseSeeding((context, _) => { Seed_AddCommunity(context); })
                          .UseAsyncSeeding(async (context, _, ct) => { await Seed_AddCommunityAsync(context, ct); });
    }
}