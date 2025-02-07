using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Extensions;

public static class WebApplicationExtensions
{
    public static void RunDatabaseMigrations(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var identityContext = app.Services.GetRequiredService<IdentityContext>();
            var dataContext = app.Services.GetRequiredService<DataContext>();
            identityContext.Database.Migrate();
            dataContext.Database.Migrate();
        }
    }
}
