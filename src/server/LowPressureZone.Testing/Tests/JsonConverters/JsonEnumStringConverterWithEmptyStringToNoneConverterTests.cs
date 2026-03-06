using System.Text.Json;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using Shouldly;
using Xunit;

namespace LowPressureZone.Testing.Tests.JsonConverters;

public sealed class JsonEnumStringConverterWithEmptyStringToNoneConverterTests
{
    [Fact]
    public void Deserialize_EmptyStringInArray_MapsToNone()
    {
        var json = """
                   {
                     "Name": "Test Playlist",
                     "Type": "default",
                     "Source": "songs",
                     "Order": "random",
                     "Weight": 1,
                     "backend_options": [""],
                     "schedule_items": []
                   }
                   """;

        var playlist = JsonSerializer.Deserialize<StationPlaylist>(json);

        playlist.ShouldNotBeNull();
        playlist.BackendOptions.ShouldBe([StationPlaylistBackendOption.None]);
    }

    [Fact]
    public void Deserialize_ValidStringsInArray_UsesStringEnumConverter()
    {
        var json = """
                   {
                     "Name": "Test Playlist",
                     "Type": "default",
                     "Source": "songs",
                     "Order": "random",
                     "Weight": 1,
                     "backend_options": ["interrupt", "single_track"],
                     "schedule_items": []
                   }
                   """;

        var playlist = JsonSerializer.Deserialize<StationPlaylist>(json);

        playlist.ShouldNotBeNull();
        playlist.BackendOptions.ShouldBe([
            StationPlaylistBackendOption.Interrupt,
            StationPlaylistBackendOption.SingleTrack
        ]);
    }

    [Fact]
    public void Serialize_None_WritesEmptyString()
    {
        var json = JsonSerializer.Serialize(new[] { StationPlaylistBackendOption.None });

        json.ShouldBe("[\"\"]");
    }
}
