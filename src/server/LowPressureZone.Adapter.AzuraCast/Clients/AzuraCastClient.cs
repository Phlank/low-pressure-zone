using System.Net.Http.Json;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Adapter.AzuraCast.Options;
using LowPressureZone.Core;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Adapter.AzuraCast.Clients;

public sealed class AzuraCastClient : IDisposable, IAzuraCastClient
{
    private readonly string _stationId;
    private readonly HttpClient _client = new();

    public AzuraCastClient(IOptions<AzuraCastConfiguration> options)
    {
        _stationId = options.Value.StationId;
        _client.BaseAddress = options.Value.ApiUrl;
        _client.DefaultRequestHeaders.Add("X-API-Key", options.Value.ApiKey);
    }

    public void Dispose() => _client.Dispose();

    private string NowPlayingEndpoint() => $"/api/nowplaying/{_stationId}";

    public async Task<Result<NowPlaying, HttpResponseMessage>> GetNowPlayingAsync()
    {
        var response = await _client.GetAsync(NowPlayingEndpoint());
        if (!response.IsSuccessStatusCode)
            return Result.Err<NowPlaying, HttpResponseMessage>(response);

        var content = await response.Content.ReadFromJsonAsync<NowPlaying>();
        if (content is null)
            return Result.Err<NowPlaying, HttpResponseMessage>(response);

        return Result.Ok<NowPlaying, HttpResponseMessage>(content);
    }

    private string StreamersEndpoint() => $"/api/station/{_stationId}/streamers";

    public async Task<Result<IReadOnlyCollection<StationStreamer>, HttpResponseMessage>> GetStreamersAsync()
    {
        var response = await _client.GetAsync(StreamersEndpoint());
        if (!response.IsSuccessStatusCode)
            return Result.Err<IReadOnlyCollection<StationStreamer>, HttpResponseMessage>(response);

        var content = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<StationStreamer>>();
        if (content is null)
            return Result.Err<IReadOnlyCollection<StationStreamer>, HttpResponseMessage>(response);

        return Result.Ok<IReadOnlyCollection<StationStreamer>, HttpResponseMessage>(content);
    }

    private string StreamerEndpoint(int streamerId) => $"/api/station/{_stationId}/streamer/{streamerId}";

    public async Task<Result<StationStreamer, HttpResponseMessage>> GetStreamerAsync(int streamerId)
    {
        var response = await _client.GetAsync(StreamerEndpoint(streamerId));
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
        var result = await _client.PostAsJsonAsync(StreamersEndpoint(), body);
        if (!result.IsSuccessStatusCode)
            return Result.Err<int, HttpResponseMessage>(result);

        var streamersResult = await GetStreamersAsync();
        var streamerId = streamersResult.Value.FirstOrDefault(streamer => streamer.StreamerUsername == username)?.Id;
        if (streamerId is null)
            return Result.Err<int, HttpResponseMessage>(streamersResult.Error);

        return Result.Ok<int, HttpResponseMessage>(streamerId.Value);
    }

    public async Task<Result<bool, HttpResponseMessage>> PutStreamerAsync(StationStreamer streamer)
    {
        var result = await _client.PutAsJsonAsync(StreamerEndpoint(streamer.Id), streamer);
        return !result.IsSuccessStatusCode
                   ? Result.Err<bool, HttpResponseMessage>(result)
                   : Result.Ok<bool, HttpResponseMessage>(true);
    }

    private string BroadcastsEndpoint(int? streamerId) => streamerId is null
                                                              ? $"/api/station/{_stationId}/streamers/broadcasts"
                                                              : $"/api/station/{_stationId}/streamer/{streamerId}/broadcasts";

    public async Task<Result<IReadOnlyCollection<StationStreamerBroadcast>, HttpResponseMessage>> GetBroadcastsAsync(
        int? streamerId = null)
    {
        var response = await _client.GetAsync(BroadcastsEndpoint(streamerId));
        if (!response.IsSuccessStatusCode)
            return Result.Err<IReadOnlyCollection<StationStreamerBroadcast>, HttpResponseMessage>(response);

        var content = await response.Content.ReadFromJsonAsync<StationStreamerBroadcast[]>();
        if (content is null)
            return Result.Err<IReadOnlyCollection<StationStreamerBroadcast>, HttpResponseMessage>(response);

        return Result.Ok<IReadOnlyCollection<StationStreamerBroadcast>, HttpResponseMessage>(content);
    }

    private string DownloadBroadcastEndpoint(int streamerId, int broadcastId) =>
        $"/api/station/{_stationId}/streamer/{streamerId}/broadcast/{broadcastId}/download";

    public async Task<Result<HttpContent, HttpResponseMessage>> DownloadBroadcastFileAsync(
        int streamerId,
        int broadcastId)
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
}