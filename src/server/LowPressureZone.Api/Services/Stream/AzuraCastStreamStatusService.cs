using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using LowPressureZone.Api.Clients;
using LowPressureZone.Api.Models.Stream;
using LowPressureZone.Api.Models.Stream.AzuraCast;

namespace LowPressureZone.Api.Services.Stream;

public class AzuraCastStreamStatusService(
    AzuraCastClient client,
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

    [SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates",
                     Justification = "Not performance sensitive")]
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

            var content = result.Value;
            lock (_statusLock)
            {
                Status = new StreamStatus
                {
                    IsOnline = content.IsOnline,
                    IsLive = content.Live.IsLive,
                    IsStatusStale = false,
                    Name = GetStreamName(content),
                    Type = "AzuraCast",
                    ListenUrl = content.Station.ListenUrl,
                    ListenerCount = content.Listeners.Current
                };
            }
        }
        catch (HttpRequestException httpRequestException)
        {
            logger.LogError(httpRequestException,
                            "Unable to retrieve status from server: The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.");
        }
        catch (TaskCanceledException taskCanceledException)
        {
            logger.LogError(taskCanceledException,
                            "Unable to retrieve status from server: The request failed due to timeout.");
        }
        catch (JsonException jsonException)
        {
            logger.LogError(jsonException,
                            "Unable to retrieve status from server: The JSON is invalid. -or- TValue is not compatible with the JSON. -or- There is remaining data in the stream.");
        }
        catch (Exception otherException)
        {
            logger.LogError(otherException, "Unable to retrieve status from server: Unspecified exception was thrown.");
        }
    }

    private static string GetStreamName(NowPlayingResponse nowPlaying)
    {
        var streamerName = nowPlaying.NowPlaying?.Streamer ?? nowPlaying.Live.StreamerName;
        if (!string.IsNullOrEmpty(streamerName))
            return streamerName;

        var songArtist = nowPlaying.NowPlaying?.Song.Artist ?? "Unknown";
        var songTitle = nowPlaying.NowPlaying?.Song.Title;
        if (songTitle is null or "" or "Live") return $"{songArtist}".Trim('-', ' ');

        return $"{songArtist} - {songTitle}".Trim('-', ' ');
    }
}
