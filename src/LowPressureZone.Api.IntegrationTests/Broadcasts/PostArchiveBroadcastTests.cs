using System.Net;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Api.Endpoints.Broadcasts.Archive;
using LowPressureZone.Api.IntegrationTests.Infrastructure;
using LowPressureZone.Core;
using static LowPressureZone.Api.IntegrationTests.Infrastructure.TestData;

namespace LowPressureZone.Api.IntegrationTests.Broadcasts;

public sealed class PostArchiveBroadcastTests(ApiFixture app) : ApiTestBase(app)
{
    private const int BroadcastId = 4;
    private const int StreamerId = 10;
    private const int ArchivePlaylistId = 42;
    private const int UploadedMediaId = 99;

    // Values from the AzuraCastInstallation section of the API's appsettings.json.
    private const string ArchivePlaylistName = "Rotation";
    private const string ArchiveDirectory = "Archive";

    private static readonly DateTime Start = new(2026, 2, 28, 22, 0, 0, DateTimeKind.Utc);

    private string? _uploadedPath;

    private Task AddLocalBroadcastAsync(bool hasFile = true, bool isArchived = false)
        => App.AddBroadcastsAsync(LocalBroadcast(BroadcastId, StreamerId, "DJ Archive", Start, Start.AddHours(1),
                                                 hasFile, isArchived));

    private void SetUpAzuraCastForArchive()
    {
        A.CallTo(() => AzuraCast.GetBroadcastsAsync(A<int?>._))
         .Returns(BroadcastsResult(RemoteBroadcast(BroadcastId, StreamerId, "DJ Archive", Start, Start.AddHours(1))));
        A.CallTo(() => AzuraCast.GetPlaylistByNameAsync(ArchivePlaylistName))
         .Returns(Result.Ok(new StationPlaylist
         {
             Id = ArchivePlaylistId,
             Name = ArchivePlaylistName,
             Type = StationPlaylistType.Default,
             Source = StationPlaylistSource.Songs,
             Order = StationPlaylistOrder.Shuffle,
             Weight = 3
         }));
        A.CallTo(() => AzuraCast.DownloadBroadcastFileAsync(StreamerId, BroadcastId))
         .ReturnsLazily(() => Ok<HttpContent>(new ByteArrayContent([1, 2, 3])));
        A.CallTo(() => AzuraCast.UploadMediaViaSftpAsync(A<Stream>._, A<string>._))
         .ReturnsLazily((Stream _, string path) =>
         {
             _uploadedPath = path;
             return Result.Ok(path);
         });
        A.CallTo(() => AzuraCast.GetMediaInDirectoryAsync(ArchiveDirectory, A<bool>._, A<bool>._))
         .ReturnsLazily(() => Ok<IEnumerable<StationFileListItem>>(
         [
             new StationFileListItem
             {
                 Path = _uploadedPath!,
                 PathShort = _uploadedPath!.Split('/').Last(),
                 Text = "",
                 Media = new StationMedia { Id = UploadedMediaId, Path = _uploadedPath }
             }
         ]));
        A.CallTo(() => AzuraCast.PutMediaAsync(UploadedMediaId, A<StationMediaRequest>._)).Returns(Ok(true));
    }

    private async Task<HttpResponseMessage> PostAsAdminAsync(int broadcastId = BroadcastId)
    {
        var admin = await App.CreateUserAsync("admin", isAdmin: true);
        return await App.CreateClientFor(admin)
                        .POSTAsync<PostArchiveBroadcast, ArchiveBroadcastRequest>(new() { Id = broadcastId });
    }

    private async Task<bool> IsArchivedAsync()
        => (await App.GetBroadcastsAsync()).Single(b => b.AzuraCastBroadcastId == BroadcastId).IsArchived;

    [Fact]
    public async Task Anonymous_user_is_unauthorized()
    {
        await AddLocalBroadcastAsync();

        var response = await App.CreateAnonymousClient()
                                .POSTAsync<PostArchiveBroadcast, ArchiveBroadcastRequest>(new() { Id = BroadcastId });

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Performer_is_forbidden()
    {
        var performer = await App.CreateUserAsync("performer", streamerId: StreamerId);
        await AddLocalBroadcastAsync();
        SetUpAzuraCastForArchive();

        var response = await App.CreateClientFor(performer)
                                .POSTAsync<PostArchiveBroadcast, ArchiveBroadcastRequest>(new() { Id = BroadcastId });

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await IsArchivedAsync()).ShouldBeFalse();
    }

    [Fact]
    public async Task Admin_archives_broadcast_into_archive_playlist()
    {
        await AddLocalBroadcastAsync();
        SetUpAzuraCastForArchive();

        var response = await PostAsAdminAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await IsArchivedAsync()).ShouldBeTrue();
        _uploadedPath.ShouldNotBeNull().ShouldStartWith($"{ArchiveDirectory}/");
        _uploadedPath.ShouldEndWith(".mp3");
        A.CallTo(() => AzuraCast.PutMediaAsync(UploadedMediaId,
                                               A<StationMediaRequest>.That.Matches(request =>
                                                   request.Artist == "DJ Archive"
                                                   && request.Title == "2026-02-28"
                                                   && request.Playlists.SequenceEqual(new[] { ArchivePlaylistId }))))
         .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Organizer_archives_broadcast()
    {
        var organizer = await App.CreateUserAsync("organizer");
        await App.MakeOrganizerAsync(organizer);
        await AddLocalBroadcastAsync();
        SetUpAzuraCastForArchive();

        var response = await App.CreateClientFor(organizer)
                                .POSTAsync<PostArchiveBroadcast, ArchiveBroadcastRequest>(new() { Id = BroadcastId });

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await IsArchivedAsync()).ShouldBeTrue();
    }

    [Fact]
    public async Task Unknown_broadcast_is_not_found()
    {
        SetUpAzuraCastForArchive();

        var response = await PostAsAdminAsync(broadcastId: 999);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        A.CallTo(() => AzuraCast.UploadMediaViaSftpAsync(A<Stream>._, A<string>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Broadcast_without_file_cannot_be_archived()
    {
        await AddLocalBroadcastAsync(hasFile: false);
        SetUpAzuraCastForArchive();

        var response = await PostAsAdminAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await IsArchivedAsync()).ShouldBeFalse();
        A.CallTo(() => AzuraCast.UploadMediaViaSftpAsync(A<Stream>._, A<string>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Archived_broadcast_cannot_be_archived_again()
    {
        await AddLocalBroadcastAsync(isArchived: true);
        SetUpAzuraCastForArchive();

        var response = await PostAsAdminAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        A.CallTo(() => AzuraCast.UploadMediaViaSftpAsync(A<Stream>._, A<string>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Broadcast_missing_from_AzuraCast_is_not_found()
    {
        await AddLocalBroadcastAsync();
        SetUpAzuraCastForArchive();
        A.CallTo(() => AzuraCast.GetBroadcastsAsync(A<int?>._)).Returns(BroadcastsResult());

        var response = await PostAsAdminAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        (await IsArchivedAsync()).ShouldBeFalse();
    }

    [Fact]
    public async Task Broadcast_without_AzuraCast_recording_is_a_bad_request()
    {
        await AddLocalBroadcastAsync();
        SetUpAzuraCastForArchive();
        A.CallTo(() => AzuraCast.GetBroadcastsAsync(A<int?>._))
         .Returns(BroadcastsResult(RemoteBroadcast(BroadcastId, StreamerId, "DJ Archive", Start, Start.AddHours(1),
                                                   hasRecording: false)));

        var response = await PostAsAdminAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))
            .ShouldContain("Broadcast recording is not available");
        (await IsArchivedAsync()).ShouldBeFalse();
    }

    [Fact]
    public async Task Missing_archive_playlist_is_a_bad_request()
    {
        await AddLocalBroadcastAsync();
        SetUpAzuraCastForArchive();
        A.CallTo(() => AzuraCast.GetPlaylistByNameAsync(ArchivePlaylistName))
         .Returns(Result.Err<StationPlaylist>("Playlist not found"));

        var response = await PostAsAdminAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))
            .ShouldContain("Unable to archive broadcast: Playlist not found");
        (await IsArchivedAsync()).ShouldBeFalse();
        A.CallTo(() => AzuraCast.UploadMediaViaSftpAsync(A<Stream>._, A<string>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Failed_media_update_does_not_mark_broadcast_archived()
    {
        await AddLocalBroadcastAsync();
        SetUpAzuraCastForArchive();
        A.CallTo(() => AzuraCast.PutMediaAsync(UploadedMediaId, A<StationMediaRequest>._))
         .Returns(Error<bool>(HttpStatusCode.InternalServerError));

        var response = await PostAsAdminAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))
            .ShouldContain("Unable to archive broadcast: Failed to update media");
        (await IsArchivedAsync()).ShouldBeFalse();
    }
}
