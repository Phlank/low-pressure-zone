using LowPressureZone.Domain;
using LowPressureZone.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDatabases(this WebApplicationBuilder builder)
    {
        var identityConnectionString = builder.Configuration.GetConnectionString("Identity");
        var databaseConnectionString = builder.Configuration.GetConnectionString("Data");
        
        builder.Services.AddDbContext<IdentityContext>(options =>
        {
            options.UseSqlite(identityConnectionString, sqliteOptions =>
            {
                sqliteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                sqliteOptions.MigrationsAssembly("LowPressureZone.Identity");
            });
        });
        builder.Services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseSqlite(databaseConnectionString, sqliteOptions =>
            {
                sqliteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                sqliteOptions.MigrationsAssembly("LowPressureZone.Domain");
            });
        });
    }
}
