using LowPressureZone.Adapter.AzuraCast.Clients;

namespace LowPressureZone.Api.IntegrationTests.Infrastructure;

/// <summary>
///     Base for API integration tests. The test host is shared, so all state is reset before each test.
/// </summary>
public abstract class ApiTestBase(ApiFixture app) : TestBase<ApiFixture>
{
    protected ApiFixture App { get; } = app;

    protected IAzuraCastClient AzuraCast => App.AzuraCast;

    protected override async ValueTask SetupAsync() => await App.ResetAsync();
}
