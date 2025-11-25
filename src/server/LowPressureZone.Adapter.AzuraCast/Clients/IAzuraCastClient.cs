using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Core;

namespace LowPressureZone.Adapter.AzuraCast.Clients;

public interface IAzuraCastClient
{
    Task<Result<NowPlaying, HttpResponseMessage>> GetNowPlayingAsync();
    
    Task<Result<IReadOnlyCollection<StationStreamer>, HttpResponseMessage>> GetStreamersAsync();
    
    Task<Result<StationStreamer, HttpResponseMessage>> GetStreamerAsync(int streamerId);

    Task<Result<int, HttpResponseMessage>> PostStreamerAsync(string username, string password, string displayName);

    Task<Result<bool, HttpResponseMessage>> PutStreamerAsync(StationStreamer streamer);

    Task<Result<IReadOnlyCollection<StationStreamerBroadcast>, HttpResponseMessage>> GetBroadcastsAsync(
        int? streamerId = null);

    Task<Result<HttpContent, HttpResponseMessage>> DownloadBroadcastFileAsync(int streamerId, int broadcastId);
    
    Task<Result<HttpContent, HttpResponseMessage>> DeleteBroadcastAsync(int streamerId, int broadcastId);

    Task<Result<string, string>> UploadMediaAsync(string targetFilePath, FileStream fileStream);

    Task<Result<IEnumerable<StationFileListItem>, HttpResponseMessage>> GetStationFilesInDirectoryAsync(
        string directory,
        bool useInternal = false,
        bool flushCache = false,
        string? searchPhrase = null);

    Task<Result<int, HttpResponseMessage>> PostPlaylistAsync(StationPlaylist playlist);
    Task<Result<bool, HttpResponseMessage>> PutMediaAsync(int mediaId, StationMediaRequest mediaRequest);
}