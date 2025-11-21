using System.Net.Http.Json;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Adapter.AzuraCast.Configuration;
using LowPressureZone.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace LowPressureZone.Adapter.AzuraCast.Clients;

public sealed class AzuraCastClient(
    HttpClient httpClient,
    IOptions<AzuraCastClientConfiguration> options,
    ISftpClient sftpClient,
    ILogger<AzuraCastClient> logger)
    : IAzuraCastClient
{
    private readonly string _stationId = options.Value.StationId;

    public async Task<Result<NowPlaying, HttpResponseMessage>> GetNowPlayingAsync()
    {
        var response = await httpClient.GetAsync(NowPlayingEndpoint());
        if (!response.IsSuccessStatusCode)
            return Result.Err<NowPlaying, HttpResponseMessage>(response);

        var content = await response.Content.ReadFromJsonAsync<NowPlaying>();
        if (content is null)
            return Result.Err<NowPlaying, HttpResponseMessage>(response);

        return Result.Ok<NowPlaying, HttpResponseMessage>(content);
    }

    public async Task<Result<IReadOnlyCollection<StationStreamer>, HttpResponseMessage>> GetStreamersAsync()
    {
        var response = await httpClient.GetAsync(StreamersEndpoint());
        if (!response.IsSuccessStatusCode)
            return Result.Err<IReadOnlyCollection<StationStreamer>, HttpResponseMessage>(response);

        var content = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<StationStreamer>>();
        if (content is null)
            return Result.Err<IReadOnlyCollection<StationStreamer>, HttpResponseMessage>(response);

        return Result.Ok<IReadOnlyCollection<StationStreamer>, HttpResponseMessage>(content);
    }

    public async Task<Result<StationStreamer, HttpResponseMessage>> GetStreamerAsync(int streamerId)
    {
        var response = await httpClient.GetAsync(StreamerEndpoint(streamerId));
        if (!response.IsSuccessStatusCode)
            return Result.Err<StationStreamer, HttpResponseMessage>(response);

        var content = await response.Content.ReadFromJsonAsync<StationStreamer>();
        if (content is null)
            return Result.Err<StationStreamer, HttpResponseMessage>(response);

        return Result.Ok<StationStreamer, HttpResponseMessage>(content);
    }

    public async Task<Result<int, HttpResponseMessage>> PostStreamerAsync(
        string username,
        string password,
        string displayName)
    {
        StationStreamer body = new()
        {
            Id = 0,
            StreamerUsername = username,
            StreamerPassword = password,
            DisplayName = displayName,
            Comments = null,
            IsActive = true,
            EnforceSchedule = false,
            ReactivateAt = null
        };
        var result = await httpClient.PostAsJsonAsync(StreamersEndpoint(), body);
        if (!result.IsSuccessStatusCode)
            return Result.Err<int, HttpResponseMessage>(result);

        var streamersResult = await GetStreamersAsync();
        if (streamersResult.IsError)
            return Result.Err<int, HttpResponseMessage>(streamersResult.Error);
        var streamerId = streamersResult.Value.FirstOrDefault(streamer => streamer.StreamerUsername == username)?.Id;
        if (streamerId is null)
            return Result.Err<int, HttpResponseMessage>(streamersResult.Error);

        return Result.Ok<int, HttpResponseMessage>(streamerId.Value);
    }

    public async Task<Result<bool, HttpResponseMessage>> PutStreamerAsync(StationStreamer streamer)
    {
        var result = await httpClient.PutAsJsonAsync(StreamerEndpoint(streamer.Id), streamer);
        return !result.IsSuccessStatusCode
                   ? Result.Err<bool, HttpResponseMessage>(result)
                   : Result.Ok<bool, HttpResponseMessage>(true);
    }

    public async Task<Result<IReadOnlyCollection<StationStreamerBroadcast>, HttpResponseMessage>> GetBroadcastsAsync(
        int? streamerId = null)
    {
        var response = await httpClient.GetAsync(BroadcastsEndpoint(streamerId));
        if (!response.IsSuccessStatusCode)
            return Result.Err<IReadOnlyCollection<StationStreamerBroadcast>, HttpResponseMessage>(response);

        var content = await response.Content.ReadFromJsonAsync<StationStreamerBroadcast[]>();
        if (content is null)
            return Result.Err<IReadOnlyCollection<StationStreamerBroadcast>, HttpResponseMessage>(response);

        return Result.Ok<IReadOnlyCollection<StationStreamerBroadcast>, HttpResponseMessage>(content);
    }

    public async Task<Result<HttpContent, HttpResponseMessage>> DownloadBroadcastFileAsync(
        int streamerId,
        int broadcastId)
    {
        var response =
            await httpClient.GetAsync(DownloadBroadcastEndpoint(streamerId, broadcastId),
                                      HttpCompletionOption.ResponseHeadersRead);
        if (!response.IsSuccessStatusCode)
            return Result.Err<HttpContent, HttpResponseMessage>(response);

        return Result.Ok<HttpContent, HttpResponseMessage>(response.Content);
    }

    public async Task<Result<HttpContent, HttpResponseMessage>> DeleteBroadcastAsync(int streamerId, int broadcastId)
    {
        var response = await httpClient.DeleteAsync(DeleteBroadcastEndpoint(streamerId, broadcastId));

        if (!response.IsSuccessStatusCode)
            return Result.Err<HttpContent, HttpResponseMessage>(response);

        return Result.Ok<HttpContent, HttpResponseMessage>(response.Content);
    }

    public async Task<Result<string, string>> UploadMediaAsync(string filePath, FileStream fileStream)
    {
        if (!sftpClient.IsConnected)
            sftpClient.Connect();

        if (await sftpClient.ExistsAsync(filePath))
            filePath = $"{filePath}_{DateTime.UtcNow.Ticks}";

        try
        {
            await sftpClient.UploadFileAsync(fileStream, filePath);
            return Result.Ok(filePath);
        }
        catch (SftpPermissionDeniedException ex)
        {
            logger.LogError(ex, "Permission denied when uploading file to AzuraCast");
            return Result.Err<string, string>(ex.Message);
        }
        catch (SshConnectionException ex)
        {
            logger.LogError(ex, "SSH connection error when uploading file to AzuraCast");
            return Result.Err<string, string>(ex.Message);
        }
        catch (SshException ex)
        {
            logger.LogError(ex, "SSH error when uploading file to AzuraCast");
            return Result.Err<string, string>(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error when uploading file to AzuraCast");
            return Result.Err<string, string>(ex.Message);
        }
    }

    private string NowPlayingEndpoint() => $"/api/nowplaying/{_stationId}";

    private string StreamersEndpoint() => $"/api/station/{_stationId}/streamers";

    private string StreamerEndpoint(int streamerId) => $"/api/station/{_stationId}/streamer/{streamerId}";

    private string BroadcastsEndpoint(int? streamerId) => streamerId is null
                                                              ? $"/api/station/{_stationId}/streamers/broadcasts"
                                                              : $"/api/station/{_stationId}/streamer/{streamerId}/broadcasts";

    private string DownloadBroadcastEndpoint(int streamerId, int broadcastId) =>
        $"/api/station/{_stationId}/streamer/{streamerId}/broadcast/{broadcastId}/download";

    private string DeleteBroadcastEndpoint(int streamerId, int broadcastId) =>
        $"/api/station/{_stationId}/streamer/{streamerId}/broadcast/{broadcastId}";
}