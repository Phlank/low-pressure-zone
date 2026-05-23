using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LowPressureZone.Domain.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public async Task<WebApplication> MigrateDataContextAsync()
        {
            await using var scope = app.Services.CreateAsyncScope();
            var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            await dataContext.Database.MigrateAsync();
            return app;
        }
    }
}