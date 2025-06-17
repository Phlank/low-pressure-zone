using LowPressureZone.Api.Models.Stream;

namespace LowPressureZone.Api.Services.Stream;

public class AzuraCastStreamStatusService(IHttpClientFactory clientFactory, IcecastStatusMapper mapper, ILogger<StreamStatusService> logger) : IStreamStatusService, IHostedService, IDisposable
{
    private readonly HttpClient _client = clientFactory.CreateClient("Icecast");
    private readonly Lock _statusLock = new();
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(10));

    public void Dispose()
    {
        _client.Dispose();
        _timer.Dispose();
    }
    public Task StartAsync(CancellationToken cancellationToken)
        => throw new NotImplementedException();
    public Task StopAsync(CancellationToken cancellationToken)
        => throw new NotImplementedException();
    public StreamStatus? Status { get; }
    public bool IsStarted { get; }
}
