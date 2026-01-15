using LowPressureZone.Api.Test.Infrastructure.Fixtures;
using Xunit;

namespace LowPressureZone.Api.Test;

[Collection("DatabaseQueryTests")]
public class AppUserClaimsTransformationTests : ICollectionFixture<DatabaseFixture>, IAsyncLifetime
{
    public ValueTask DisposeAsync()
    { 
        return ValueTask.CompletedTask; 
    }

    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
