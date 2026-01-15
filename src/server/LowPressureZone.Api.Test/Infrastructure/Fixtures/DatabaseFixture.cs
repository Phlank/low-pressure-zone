using LowPressureZone.Domain;
using LowPressureZone.Identity;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace LowPressureZone.Api.Test;

public class DatabaseFixture : IAsyncLifetime
{
    private bool _isInitialized = false;
    private DataContext? _dataContext;
    private readonly PostgreSqlContainer _container;
    private readonly string _connectionString;

    public DatabaseFixture()
    {
        _container = new PostgreSqlBuilder("postgres:16-alpine").Build();
        _connectionString = _container.GetConnectionString();
    }

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();
        var dataContextOptions = new DbContextOptionsBuilder<DataContext>().UseNpgsql(_connectionString).Options;
        _dataContext = new DataContext(dataContextOptions);
        await _dataContext.Database.EnsureCreatedAsync();
        await _dataContext.Database.MigrateAsync();
        _isInitialized = true;
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

    public DataContext DataContext => _isInitialized ? _dataContext! : throw new InvalidOperationException("DatabaseFixture is not initialized.");
}