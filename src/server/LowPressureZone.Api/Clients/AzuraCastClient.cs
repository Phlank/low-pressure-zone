using LowPressureZone.Api.Models;
using LowPressureZone.Api.Models.Options;
using LowPressureZone.Api.Models.Stream.AzuraCast;
using LowPressureZone.Api.Models.Stream.AzuraCast.Schema;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Api.Clients;

public class AzuraCastClient(IHttpClientFactory clientFactory, IOptions<AzuraCastOptions> options)
{
    private readonly HttpClient _client = clientFactory.CreateClient("AzuraCast");
    private readonly string _stationId = options.Value.StationId;

    public async Task<Result<NowPlayingResponse, HttpResponseMessage>> GetNowPlayingAsync()
    {
        var response = await _client.GetAsync($"/api/nowplaying/{_stationId}");
        if (!response.IsSuccessStatusCode)
            return Result<NowPlayingResponse, HttpResponseMessage>.Err(response);

        var content = await response.Content.ReadFromJsonAsync<NowPlayingResponse>();
        if (content is null)
            return Result<NowPlayingResponse, HttpResponseMessage>.Err(response);

        return Result<NowPlayingResponse, HttpResponseMessage>.Ok(content);
    }

    public async Task<Result<IReadOnlyCollection<Streamer>, HttpResponseMessage>> GetStreamersAsync()
    {
        var response = await _client.GetAsync($"/api/station/{_stationId}/streamers");
        if (!response.IsSuccessStatusCode)
            return Result<IReadOnlyCollection<Streamer>, HttpResponseMessage>.Err(response);

        var content = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<Streamer>>();
        if (content is null)
            return Result<IReadOnlyCollection<Streamer>, HttpResponseMessage>.Err(response);

        return Result<IReadOnlyCollection<Streamer>, HttpResponseMessage>.Ok(content);
    }

    public async Task<Result<Streamer, HttpResponseMessage>> GetStreamerAsync(int streamerId)
    {
        var response = await _client.GetAsync($"/api/station/{_stationId}/streamer/{streamerId}");
        if (!response.IsSuccessStatusCode)
            return Result<Streamer, HttpResponseMessage>.Err(response);

        var content = await response.Content.ReadFromJsonAsync<Streamer>();
        if (content is null)
            return Result<Streamer, HttpResponseMessage>.Err(response);

        return Result<Streamer, HttpResponseMessage>.Ok(content);
    }

    public async Task<Result<int, HttpResponseMessage>> CreateStreamerAsync(string username, string password, string displayName)
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
        var result = await _client.PostAsJsonAsync($"/api/station/{_stationId}/streamers", body);
        if (!result.IsSuccessStatusCode) return Result<int, HttpResponseMessage>.Err(result);

        var streamers = await GetStreamersAsync();
        var streamerId = streamers.Data?.FirstOrDefault(streamer => streamer.StreamerUsername == username)?.Id;
        if (streamerId is null)
            return Result<int, HttpResponseMessage>.Err(streamers.Error!);

        return Result<int, HttpResponseMessage>.Ok(streamerId.Value);
    }

    public async Task<Result<bool, HttpResponseMessage>> UpdateStreamerAsync(Streamer streamer)
    {
        var result = await _client.PutAsJsonAsync($"/api/station/{_stationId}/streamer/{streamer.Id}", streamer);
        return !result.IsSuccessStatusCode
            ? Result<bool, HttpResponseMessage>.Err(result)
            : Result<bool, HttpResponseMessage>.Ok(true);
    }

    public async Task<Result<Broadcast[], HttpResponseMessage>> GetBroadcastsAsync(int? streamerId = null)
    {
        var endpoint = streamerId is null
            ? $"/api/station/{_stationId}/streamers/broadcasts"
            : $"/api/station/{_stationId}/streamer/{streamerId}/broadcasts";

        var response = await _client.GetAsync(endpoint);
        if (!response.IsSuccessStatusCode)
            return Result<Broadcast[], HttpResponseMessage>.Err(response);

        var content = await response.Content.ReadFromJsonAsync<Broadcast[]>();
        if (content is null)
            return Result<Broadcast[], HttpResponseMessage>.Err(response);

        return Result<Broadcast[], HttpResponseMessage>.Ok(content);
    }

    public async Task<Result<HttpContent, HttpResponseMessage>> DownloadBroadcastAsync(int streamerId, int broadcastId)
    {
        var response = await _client.GetAsync($"/api/station/{_stationId}/streamer/{streamerId}/broadcast/{broadcastId}/download", HttpCompletionOption.ResponseHeadersRead);
        if (!response.IsSuccessStatusCode)
            return Result<HttpContent, HttpResponseMessage>.Err(response);

        return Result<HttpContent, HttpResponseMessage>.Ok(response.Content);
    }
}
