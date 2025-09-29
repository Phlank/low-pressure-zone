using System.Text.Json;
using LowPressureZone.Api.Models;
using LowPressureZone.Api.Models.Options;
using LowPressureZone.Api.Models.Stream;
using LowPressureZone.Api.Models.Stream.AzuraCast;
using LowPressureZone.Api.Models.Stream.AzuraCast.Schema;
using LowPressureZone.Domain.Entities;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace LowPressureZone.Api.Clients;

public class AzuraCastClient(
    IHttpClientFactory clientFactory,
    IOptions<StreamingOptions> options,
    ISftpClient sftpClient,
    ILogger<AzuraCastClient> logger)
{
    private readonly HttpClient _client = clientFactory.CreateClient(nameof(StreamServerType.AzuraCast));

    private readonly string _mediaDirectory = options.Value
                                                     .Streams
                                                     .First(stream => stream.Server == StreamServerType.AzuraCast)
                                                     .AzuraCast!.MediaDirectory;

    private readonly string _stationId = options.Value
                                                .Streams
                                                .First(stream => stream.Server == StreamServerType.AzuraCast)
                                                .AzuraCast!.StationId;

    private string NowPlayingEndpoint() => $"/api/nowplaying/{_stationId}";

    public async Task<Result<NowPlayingResponse, HttpResponseMessage>> GetNowPlayingAsync()
    {
        var response = await _client.GetAsync(NowPlayingEndpoint());
        if (!response.IsSuccessStatusCode)
            return Result.Err<NowPlayingResponse, HttpResponseMessage>(response);

        var content = await response.Content.ReadFromJsonAsync<NowPlayingResponse>();
        if (content is null)
            return Result.Err<NowPlayingResponse, HttpResponseMessage>(response);

        return Result.Ok<NowPlayingResponse, HttpResponseMessage>(content);
    }

    private string StreamersEndpoint() => $"/api/station/{_stationId}/streamers";

    public async Task<Result<IReadOnlyCollection<Streamer>, HttpResponseMessage>> GetStreamersAsync()
    {
        var response = await _client.GetAsync(StreamersEndpoint());
        if (!response.IsSuccessStatusCode)
            return Result.Err<IReadOnlyCollection<Streamer>, HttpResponseMessage>(response);

        var content = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<Streamer>>();
        if (content is null)
            return Result.Err<IReadOnlyCollection<Streamer>, HttpResponseMessage>(response);

        return Result.Ok<IReadOnlyCollection<Streamer>, HttpResponseMessage>(content);
    }

    private string StreamerEndpoint(int streamerId) => $"/api/station/{_stationId}/streamer/{streamerId}";

    public async Task<Result<Streamer, HttpResponseMessage>> GetStreamerAsync(int streamerId)
    {
        var response = await _client.GetAsync(StreamerEndpoint(streamerId));
        if (!response.IsSuccessStatusCode)
            return Result.Err<Streamer, HttpResponseMessage>(response);

        var content = await response.Content.ReadFromJsonAsync<Streamer>();
        if (content is null)
            return Result.Err<Streamer, HttpResponseMessage>(response);

        return Result.Ok<Streamer, HttpResponseMessage>(content);
    }

    public async Task<Result<int, HttpResponseMessage>> CreateStreamerAsync(
        string username, string password, string displayName)
    {
        Streamer body = new()
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
        var result = await _client.PostAsJsonAsync(StreamersEndpoint(), body);
        if (!result.IsSuccessStatusCode)
            return Result.Err<int, HttpResponseMessage>(result);

        var streamersResult = await GetStreamersAsync();
        var streamerId = streamersResult.Value.FirstOrDefault(streamer => streamer.StreamerUsername == username)?.Id;
        if (streamerId is null)
            return Result.Err<int, HttpResponseMessage>(streamersResult.Error);

        return Result.Ok<int, HttpResponseMessage>(streamerId.Value);
    }

    public async Task<Result<bool, HttpResponseMessage>> UpdateStreamerAsync(Streamer streamer)
    {
        var result = await _client.PutAsJsonAsync(StreamerEndpoint(streamer.Id), streamer);
        return !result.IsSuccessStatusCode
                   ? Result.Err<bool, HttpResponseMessage>(result)
                   : Result.Ok<bool, HttpResponseMessage>(true);
    }

    private string BroadcastsEndpoint(int? streamerId) => streamerId is null
                                                              ? $"/api/station/{_stationId}/streamers/broadcasts"
                                                              : $"/api/station/{_stationId}/streamer/{streamerId}/broadcasts";

    public async Task<Result<IReadOnlyCollection<Broadcast>, HttpResponseMessage>> GetBroadcastsAsync(
        int? streamerId = null)
    {
        var response = await _client.GetAsync(BroadcastsEndpoint(streamerId));
        if (!response.IsSuccessStatusCode)
            return Result.Err<IReadOnlyCollection<Broadcast>, HttpResponseMessage>(response);

        var content = await response.Content.ReadFromJsonAsync<Broadcast[]>();
        if (content is null)
            return Result.Err<IReadOnlyCollection<Broadcast>, HttpResponseMessage>(response);

        return Result.Ok<IReadOnlyCollection<Broadcast>, HttpResponseMessage>(content);
    }

    private string DownloadBroadcastEndpoint(int streamerId, int broadcastId) =>
        $"/api/station/{_stationId}/streamer/{streamerId}/broadcast/{broadcastId}/download";

    public async Task<Result<HttpContent, HttpResponseMessage>> DownloadBroadcastAsync(int streamerId, int broadcastId)
    {
        var response =
            await _client.GetAsync(DownloadBroadcastEndpoint(streamerId, broadcastId),
                                   HttpCompletionOption.ResponseHeadersRead);
        if (!response.IsSuccessStatusCode)
            return Result.Err<HttpContent, HttpResponseMessage>(response);

        return Result.Ok<HttpContent, HttpResponseMessage>(response.Content);
    }

    private string DeleteBroadcastEndpoint(int streamerId, int broadcastId) =>
        $"/api/station/{_stationId}/streamer/{streamerId}/broadcast/{broadcastId}";

    public async Task<Result<HttpContent, HttpResponseMessage>> DeleteBroadcastAsync(int streamerId, int broadcastId)
    {
        var response = await _client.DeleteAsync(DeleteBroadcastEndpoint(streamerId, broadcastId));

        if (!response.IsSuccessStatusCode)
            return Result.Err<HttpContent, HttpResponseMessage>(response);

        return Result.Ok<HttpContent, HttpResponseMessage>(response.Content);
    }

    public async Task<Result<string, string>> UploadMediaAsync(string filePath, IFormFile file)
    {
        if (!sftpClient.IsConnected)
            sftpClient.Connect();

        if (await sftpClient.ExistsAsync(filePath)) filePath = $"{filePath}_{DateTime.UtcNow.Ticks}";

        try
        {
            await using (var stream = file.OpenReadStream())
            {
                sftpClient.UploadFile(stream, filePath);
            }

            return Result.Ok<string, string>(filePath);
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

    public async Task<Result<HttpContent, HttpResponseMessage>> CreatePrerecordedItemAsync(
        int streamerId, Timeslot timeslot, IFormFile file)
    {

        var playlist = CreatePlaylistForFile(file, streamerId);

        var createPlaylistResult = await _client.PostAsJsonAsync($"/api/station/{_stationId}/playlists", playlist);
        if (!createPlaylistResult.IsSuccessStatusCode)
        {
            logger.LogError("Failed to create playlist; {Reason}", createPlaylistResult.ReasonPhrase);
            logger.LogError("Content: {Content}", await createPlaylistResult.Content.ReadAsStringAsync());
            logger.LogError("Playlist being posted: {Playlist}", JsonSerializer.Serialize(playlist));
            return Result.Err<HttpContent, HttpResponseMessage>(createPlaylistResult);
        }

        var uploadResult = await UploadMediaAsync(file.FileName, file);
        if (!uploadResult.IsSuccess)
        {
            logger.LogError("Failed to upload media; {Reason}", uploadResult.Error);
            var deletePlaylistResult = await _client.DeleteAsync($"/api/station/{_stationId}/playlists");
        }

        throw new NotImplementedException();
    }

    private async Task<Result<StationPlaylist, string>> CreatePlaylistForFileAsync(IFormFile file, int streamerId)
    {
        var streamerResult = await GetStreamerAsync(streamerId);
        if (!streamerResult.IsSuccess)
            return Result.Err<StationPlaylist, string>(streamerResult.Error.ReasonPhrase);
        StationPlaylist playlist = new()
        {
            Name = streamerResult.Value.DisplayName!,
            Order = PlaylistOrders.Sequential,
            Source = PlaylistSources.Songs,
            Types = PlaylistTypes.Default,
            AvoidDuplicates = false,
            BackendOptions =
            [
                PlaylistBackendOptionTypes.Interrupt,
                PlaylistBackendOptionTypes.LoopOnce,
                PlaylistBackendOptionTypes.Merge
            ],
            IncludeInOnDemand = false,
            IncludeInRequests = false,
            IsEnabled = true,
            IsJingle = false,
            PlayPerHourMinute = 0,
            PlayPerMinutes = 0,
            PlayPerSongs = 0,
            RemoteBuffer = 0,
            Weight = 1
        };
        if (timeslot.StartsAt.Date < DateTime.UtcNow.Date.AddDays(-6))
        {
            List<int> days =
            [
                (int)timeslot.StartsAt.DayOfWeek,
                (int)timeslot.EndsAt.DayOfWeek
            ];
            playlist.ScheduleItems = new StationSchedule
            {
                Days = days.Distinct(),
                StartTime = timeslot.StartsAt.Hour * 100 + timeslot.StartsAt.Minute,
                EndTime = timeslot.EndsAt.Hour * 100 + timeslot.EndsAt.Minute
            };
        }
    }
}
