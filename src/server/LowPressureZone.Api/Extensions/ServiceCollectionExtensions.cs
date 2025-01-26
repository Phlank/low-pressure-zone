using LowPressureZone.Domain;
using LowPressureZone.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDatabases(this WebApplicationBuilder builder)
    {
        var identityConnectionString = builder.Configuration.GetConnectionString("Identity");
        var dataConnectionString = builder.Configuration.GetConnectionString("Data");
        
        builder.Services.AddDbContext<IdentityContext>(options =>
        {
            options.UseNpgsql(identityConnectionString);
        });
        builder.Services.AddDbContext<DataContext>(options =>
        {
            options.UseNpgsql(dataConnectionString);
        });
    }
}
