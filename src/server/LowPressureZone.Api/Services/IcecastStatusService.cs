using System.Text.Json;
using LowPressureZone.Api.Models.Icecast;
using IHttpClientFactory = System.Net.Http.IHttpClientFactory;

namespace LowPressureZone.Api.Services;

public class IcecastStatusService(IHttpClientFactory clientFactory, ILogger<IcecastStatusService> logger) : IHostedService, IDisposable
{
    private const string StatusEndpoint = "/status-json.xsl";
    private readonly HttpClient _client = clientFactory.CreateClient("Icecast");
    private readonly Lock _statusLock = new();
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(10));
    private IcecastStatusRaw? _icecastStatus;

    public IcecastStatusRaw? Status
    {
        get
        {
            lock (_statusLock)
            {
                return _icecastStatus;
            }
        }
        private set
        {
            lock (_statusLock)
            {
                _icecastStatus = value;
            }
        }
    }

    public bool IsStarted { get; private set; }

    public void Dispose()
        => _timer.Dispose();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (IsStarted) return;
        IsStarted = true;
        await RefreshStatusAsync(cancellationToken);
        _ = ContinuallyRefreshStatusAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        IsStarted = false;
        return Task.CompletedTask;
    }

    private async Task RefreshStatusAsync(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _client.GetAsync(StatusEndpoint, cancellationToken);
            if (!result.IsSuccessStatusCode)
            {
                Status = null;
                logger.LogWarning($"{nameof(IcecastStatusService)}: Unable to retrieve status from Icecast service | {{Status}} | {{Reason}}", result.StatusCode, result.ReasonPhrase);
                return;
            }
            await using var contentStream = await result.Content.ReadAsStreamAsync(cancellationToken);
            var content = await JsonSerializer.DeserializeAsync<IcecastStatusRaw>(contentStream, JsonSerializerOptions.Web, cancellationToken);
            Status = content;
        }
        catch (HttpRequestException httpRequestException)
        {
            logger.LogError(httpRequestException, "Unable to retrieve status from Icecast server");
        }
        catch (JsonException jsonException)
        {
            logger.LogError(jsonException, "Unable to retrieve status from Icecast server");
        }
    }

    private async Task ContinuallyRefreshStatusAsync(CancellationToken cancellationToken)
    {
        while (await _timer.WaitForNextTickAsync(cancellationToken) && IsStarted)
            await RefreshStatusAsync(cancellationToken);
    }
}
