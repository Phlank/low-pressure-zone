using LowPressureZone.Api.Models.Stream;
using LowPressureZone.Api.Services.StreamStatus;

namespace LowPressureZone.Api.IntegrationTests.Infrastructure;

public sealed class TestStreamStatusService : IStreamStatusService
{
    public StreamStatus? Status { get; set; }
    public bool IsStarted { get; private set; }
    public int StartCount { get; private set; }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        IsStarted = true;
        StartCount++;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        IsStarted = false;
        return Task.CompletedTask;
    }

    public void Reset()
    {
        Status = null;
        IsStarted = false;
        StartCount = 0;
    }
}
