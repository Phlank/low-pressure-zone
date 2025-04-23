using System.Text.Json;
using LowPressureZone.Api.Models.Icecast;
using IHttpClientFactory = System.Net.Http.IHttpClientFactory;

namespace LowPressureZone.Api.Services;

public class IcecastStatusService(IHttpClientFactory clientFactory, ILogger<IcecastStatusService> logger) : IHostedService, IDisposable
{
    private const string statusEndpoint = "/status-json.xsl";
    private readonly HttpClient _client = clientFactory.CreateClient("Icecast");
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(10));
    public IcecastStatusRaw? Status { get; private set; }
    public bool IsStarted { get; private set; }

    public void Dispose()
        => _timer.Dispose();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        IsStarted = true;
        while (await _timer.WaitForNextTickAsync(cancellationToken) && IsStarted)
        {
            var result = await _client.GetAsync(statusEndpoint, cancellationToken);
            if (!result.IsSuccessStatusCode)
            {
                Status = null;
                logger.LogWarning($"{nameof(IcecastStatusService)}: Unable to retrieve status from Icecast service | {{Status}} | {{Reason}}", result.StatusCode, result.ReasonPhrase);
                continue;
            }
            await using var contentStream = await result.Content.ReadAsStreamAsync(cancellationToken);
            var content = await JsonSerializer.DeserializeAsync<IcecastStatusRaw>(contentStream, JsonSerializerOptions.Web, cancellationToken);
            Status = content;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        IsStarted = false;
        return Task.CompletedTask;
    }
}
