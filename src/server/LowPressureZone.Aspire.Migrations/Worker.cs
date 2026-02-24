using System.Diagnostics;
using LowPressureZone.Domain;
using LowPressureZone.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Aspire.Migrations;

public sealed class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(nameof(Migrations), ActivityKind.Client);
        using var scope = serviceProvider.CreateScope();
        
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();

        await dataContext.Database.MigrateAsync(cancellationToken);
        await identityContext.Database.MigrateAsync(cancellationToken);

        hostApplicationLifetime.StopApplication();
    }
}