namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public class FileListDirectory
{
    public IEnumerable<StationMediaPlaylist> Playlists { get; set; } = [];
}