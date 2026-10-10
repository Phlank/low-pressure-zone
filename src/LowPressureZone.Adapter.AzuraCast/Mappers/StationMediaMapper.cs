using LowPressureZone.Adapter.AzuraCast.ApiSchema;

namespace LowPressureZone.Adapter.AzuraCast.Mappers;

public static class StationMediaMapper
{
    public static StationMediaRequest ToRequest(StationMedia media) =>
        new()
        {
            Album = media.Album,
            Artist = media.Artist,
            Title = media.Title,
            Genre = media.Genre,
            ExtraMetadata = media.ExtraMetadata,
            Isrc = media.Isrc,
            Lyrics = media.Lyrics,
            Path = media.Path,
            Playlists = media.Playlists.Select(playlist => playlist.Id)
        };
}