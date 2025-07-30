using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using LowPressureZone.Api.Models.Stream;
using LowPressureZone.Api.Models.Stream.Icecast;
using Shouldly;

namespace LowPressureZone.Api.Services.Stream;

public class IcecastStatusService(
    IHttpClientFactory clientFactory,
    IcecastStatusMapper mapper,
    ILogger<IcecastStatusService> logger) : IStreamStatusService, IDisposable
{
    private const string StatusEndpoint = "/status-json.xsl";
    private readonly HttpClient _client = clientFactory.CreateClient("Icecast");
    private readonly Lock _statusLock = new();
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(10));
    private IcecastStatusRaw? _icecastStatus;

    public void Dispose()
    {
        _timer.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (IsStarted) return;
        IsStarted = true;
        await RefreshStatusAsync();
        _ = ContinuallyRefreshStatusAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        IsStarted = false;
        return Task.CompletedTask;
    }

    public StreamStatus Status
    {
        get
        {
            lock (_statusLock)
            {
                return mapper.FromEntity(_icecastStatus);
            }
        }
    }

    public bool IsStarted { get; private set; }

    private async Task ContinuallyRefreshStatusAsync()
    {
        while (await _timer.WaitForNextTickAsync() && IsStarted)
            await RefreshStatusAsync();
    }

    // Sets the status.
    // If the icecast server is offline, then this service will retain the last held data.
    // An IcecastStatusRaw instance becomes stale 30 seconds after it is created.
    [SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates",
                     Justification = "Not performance sensitive")]
    private async Task RefreshStatusAsync()
    {
        try
        {
            var result = await _client.GetAsync(StatusEndpoint);
            if (!result.IsSuccessStatusCode)
            {
                logger.LogWarning($"{nameof(IcecastStatusService)}: Unable to retrieve status from Icecast service | {{Status}} | {{Reason}}",
                                  result.StatusCode, result.ReasonPhrase);
                return;
            }

            await using var contentStream = await result.Content.ReadAsStreamAsync();
            var content =
                await JsonSerializer.DeserializeAsync<IcecastStatusRootRaw>(contentStream, JsonSerializerOptions.Web);
            content.ShouldNotBeNull();
            lock (_statusLock)
            {
                _icecastStatus = content.Stats;
            }
        }
        catch (HttpRequestException httpRequestException)
        {
            logger.LogError(httpRequestException,
                            "Unable to retrieve status from Icecast server: The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.");
        }
        catch (TaskCanceledException taskCanceledException)
        {
            logger.LogError(taskCanceledException,
                            "Unable to retrieve status from Icecast server: The request failed due to timeout.");
        }
        catch (JsonException jsonException)
        {
            logger.LogError(jsonException,
                            "Unable to retrieve status from Icecast server: The JSON is invalid. -or- TValue is not compatible with the JSON. -or- There is remaining data in the stream.");
        }
        catch (Exception otherException)
        {
            logger.LogError(otherException,
                            "Unable to retrieve status from Icecast server: Unspecified exception was thrown.");
        }
    }
}