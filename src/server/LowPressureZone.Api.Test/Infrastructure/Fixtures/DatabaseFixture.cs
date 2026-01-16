using LowPressureZone.Api.Test.Tests;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace LowPressureZone.Api.Test.Infrastructure.Fixtures;

[Collection("DatabaseQueryTests")]
public class DatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:16-alpine").Build();
    private DataContext? _dataContext;
    private bool _isInitialized;

    public DataContext DataContext => _isInitialized
                                          ? _dataContext!
                                          : throw new InvalidOperationException("DatabaseFixture is not initialized.");

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();
        var connectionString = _container.GetConnectionString();
        var dataContextOptions = new DbContextOptionsBuilder<DataContext>().UseNpgsql(connectionString).Options;
        _dataContext = new DataContext(dataContextOptions);
        await _dataContext.Database.MigrateAsync();
        await AddDataAsync();
        _isInitialized = true;
    }
    
    private async Task AddDataAsync()
    {
        if (_dataContext is null)
            throw new InvalidOperationException("DataContext is not initialized.");

        _dataContext.AddRange(AppUserClaimsTransformationTestsData.Communities);

        await _dataContext.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (!_isInitialized)
            throw new InvalidOperationException("DatabaseFixture is not initialized.");

        _isInitialized = false;
        if (_dataContext is not null)
            await _dataContext.DisposeAsync();
        await _container.DisposeAsync();
    }
}