using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LowPressureZone.Identity.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public async Task<WebApplication> MigrateIdentityContextAsync()
        {
            await using var scope = app.Services.CreateAsyncScope();
            var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
            await identityContext.Database.MigrateAsync();
            await identityContext.SeedRolesAsync(CancellationToken.None);
            await identityContext.SeedAdminUserAsync(CancellationToken.None);
            return app;
        }
    }
}