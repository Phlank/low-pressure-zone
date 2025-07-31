using LowPressureZone.Api.Models;
using LowPressureZone.Api.Models.Options;
using LowPressureZone.Api.Models.Stream;
using LowPressureZone.Api.Models.Stream.AzuraCast;
using LowPressureZone.Api.Models.Stream.AzuraCast.Schema;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Api.Clients;

public class AzuraCastClient(IHttpClientFactory clientFactory, IOptions<StreamingOptions> options)
{
    private readonly HttpClient _client = clientFactory.CreateClient(nameof(StreamServerType.AzuraCast));

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

    public async Task<Result<Broadcast[], HttpResponseMessage>> GetBroadcastsAsync(int? streamerId = null)
    {
        var response = await _client.GetAsync(BroadcastsEndpoint(streamerId));
        if (!response.IsSuccessStatusCode)
            return Result.Err<Broadcast[], HttpResponseMessage>(response);

        var content = await response.Content.ReadFromJsonAsync<Broadcast[]>();
        if (content is null)
            return Result.Err<Broadcast[], HttpResponseMessage>(response);

        return Result.Ok<Broadcast[], HttpResponseMessage>(content);
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
}
