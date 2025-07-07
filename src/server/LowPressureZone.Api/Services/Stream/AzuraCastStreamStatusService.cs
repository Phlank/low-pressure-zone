using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using LowPressureZone.Api.Clients;
using LowPressureZone.Api.Models.Stream;

namespace LowPressureZone.Api.Services.Stream;

public class AzuraCastStreamStatusService(AzuraCastClient client,
                                          ILogger<IcecastStatusService> logger)
    : IStreamStatusService, IDisposable
{
    private readonly Lock _statusLock = new();
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(5));

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

    public StreamStatus? Status { get; private set; }

    public bool IsStarted { get; private set; }

    private async Task ContinuallyRefreshStatusAsync()
    {
        while (await _timer.WaitForNextTickAsync() && IsStarted)
            await RefreshStatusAsync();
    }

    [SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "Not performance sensitive")]
    private async Task RefreshStatusAsync()
    {
        try
        {
            var result = await client.GetNowPlayingAsync();
            if (!result.IsSuccess)
            {
                logger.LogError($"{nameof(AzuraCastStreamStatusService)}: Unable to retrieve status from AzuraCast");
                return;
            }
            var content = result.Data;
            if (content is null)
                return;

            var name = content.NowPlaying?.Song.Artist ?? "Unknown";

            if (content.NowPlaying?.Song.Title == "Live")
                content.NowPlaying.Song.Title = null;

            if (content.NowPlaying?.Song.Title is not null)
                name += $" - {content.NowPlaying.Song.Title}";

            name = name.Trim(['-', ' ']);

            lock (_statusLock)
            {
                Status = new StreamStatus
                {
                    IsOnline = content.IsOnline,
                    IsLive = content.Live.IsLive,
                    IsStatusStale = false,
                    Name = name,
                    Type = "AzuraCast",
                    ListenUrl = content.Station.ListenUrl,
                    ListenerCount = content.Listeners.Current
                };
            }
        }
        catch (HttpRequestException httpRequestException)
        {
            logger.LogError(httpRequestException, "Unable to retrieve status from server: The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.");
        }
        catch (TaskCanceledException taskCanceledException)
        {
            logger.LogError(taskCanceledException, "Unable to retrieve status from server: The request failed due to timeout.");
        }
        catch (JsonException jsonException)
        {
            logger.LogError(jsonException, "Unable to retrieve status from server: The JSON is invalid. -or- TValue is not compatible with the JSON. -or- There is remaining data in the stream.");
        }
        catch (Exception otherException)
        {
            logger.LogError(otherException, "Unable to retrieve status from server: Unspecified exception was thrown.");
        }
    }
}
