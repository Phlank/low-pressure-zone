using LowPressureZone.Data;
using LowPressureZone.Identity;
using LowPressureZone.Testing.Tests.Authentication;
using LowPressureZone.Testing.Tests.Endpoints.Communities;
using Microsoft.EntityFrameworkCore;
using Xunit;

// ReSharper disable ArrangeAccessorOwnerBody

namespace LowPressureZone.Testing.Infrastructure.Fixtures;

[Collection("Database Query Tests")]
public class DatabaseFixture : IAsyncLifetime
{
    private DataContext? _dataContext;
    private IdentityContext? _identityContext;
    private bool _isInitialized;

    public DataContext DataContext
    {
        get
        {
            return _isInitialized
                       ? _dataContext!
                       : throw new InvalidOperationException("DatabaseFixture is not initialized.");
        }
    }

    public IdentityContext IdentityContext
    {
        get
        {
            return _isInitialized
                       ? _identityContext!
                       : throw new InvalidOperationException("DatabaseFixture is not initialized.");
        }
    }

    public async ValueTask InitializeAsync()
    {
        var dataContextOptions = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase("lpz-data")
                                                                           .Options;
        var identityContextOptions = new DbContextOptionsBuilder<IdentityContext>().UseInMemoryDatabase("lpz-identity")
                                                                                   .Options;
        _dataContext = new DataContext(dataContextOptions);
        _identityContext = new IdentityContext(identityContextOptions);
        await _dataContext.Database.EnsureCreatedAsync();
        await _identityContext.Database.EnsureCreatedAsync();
        await AddDataAsync();
        _isInitialized = true;
    }

    private async Task AddDataAsync()
    {
        if (_dataContext is null)
            throw new InvalidOperationException("DataContext is not initialized.");

        _dataContext.AddRange(AppUserClaimsTransformationTestsData.Communities);
        _dataContext.AddRange(CommunityRequestValidatorTestsData.Communities);

        await _dataContext.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (!_isInitialized)
            throw new InvalidOperationException("DatabaseFixture is not initialized.");

        _isInitialized = false;
        if (_dataContext is not null)
            await _dataContext.DisposeAsync();
        if (_identityContext is not null)
            await _identityContext.DisposeAsync();
    }
}