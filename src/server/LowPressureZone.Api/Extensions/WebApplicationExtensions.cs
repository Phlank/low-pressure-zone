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

    public static IApplicationBuilder UseRedirectUnauthorizedToChallengeEndpoint(this WebApplication webApplication)
    {
        webApplication.Use(async (context, next) =>
        {
            await next.Invoke();

            var isRedirect = context.Response.StatusCode == 302;
            var isOauth2 = context.Response.Headers.Location.Any(l => l.Contains("oauth2"));
            var isChallenge = context.Request.Path == "/challenge";
            if (isRedirect && isOauth2 && !isChallenge)
            {
                context.Response.Headers.Append("Require-Login", "Yes");
                context.Response.Headers.AccessControlExposeHeaders = "Require-Login";
                await context.Response.SendUnauthorizedAsync();
            }
        });
        return webApplication;
    }
}
