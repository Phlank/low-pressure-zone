using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Adapter.AzuraCast.Clients;

namespace LowPressureZone.Api.Services.StreamStatus;

public sealed class AzuraCastStatusService(
    IAzuraCastClient client,
    ILogger<AzuraCastStatusService> logger)
    : IStreamStatusService, IDisposable
{
    private readonly Lock _statusLock = new();
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(5));
    
    public Models.Stream.StreamStatus? Status { get; private set; }

    public void Dispose() => _timer.Dispose();

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
                logger.LogError($"{nameof(AzuraCastStatusService)}: Unable to retrieve status from AzuraCast");
                return;
            }

            var content = result.Value;
            lock (_statusLock)
            {
                Status = new Models.Stream.StreamStatus
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

    /// <summary>
    ///     Stream name is either the streamer name (if live) or "Artist - Title".
    /// </summary>
    /// <param name="nowPlaying"></param>
    /// <returns></returns>
    private static string GetStreamName(NowPlaying nowPlaying)
    {
        var streamerName = nowPlaying.Live.StreamerName;
        if (string.IsNullOrEmpty(streamerName))
            streamerName = nowPlaying.CurrentSong?.Streamer;

        if (!string.IsNullOrEmpty(streamerName))
            return streamerName;

        var songArtist = nowPlaying.CurrentSong?.Song.Artist ?? "Unknown";
        var songTitle = nowPlaying.CurrentSong?.Song.Title;
        if (songTitle is null or "" or "Live")
            return $"{songArtist}".Trim('-', ' ');

        return $"{songArtist} - {songTitle}".Trim('-', ' ');
    }
}