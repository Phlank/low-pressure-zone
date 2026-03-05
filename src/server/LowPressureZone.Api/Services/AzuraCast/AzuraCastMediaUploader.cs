using System.Text;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Api.Models.Configuration;
using LowPressureZone.Core;
using Microsoft.Extensions.Options;
using Shouldly;

namespace LowPressureZone.Api.Services.AzuraCast;

public partial class AzuraCastMediaUploader(
    IAzuraCastClient client,
    IOptions<AzuraCastInstallationConfiguration> installation,
    ILogger<AzuraCastMediaUploader> logger)
{
    public async Task<Result<StationMedia, string>> UploadAndGetMediaAsync(
        Stream stream,
        AzuraCastMediaDirectory directory)
    {
        var path = GetFilePath(directory);
        var uploadResult = await client.UploadMediaViaSftpAsync(stream, path);
        if (uploadResult.IsError)
            return Result.Err<StationMedia>("Unable to upload media.");

        var uploadedFileResult = await Retry.RetryAsync(async () => await GetUploadedFileAsync(path, directory),
                                                        result => result.IsError
                                                                  || (result.IsSuccess &&
                                                                      result.Value.Media is not null),
                                                        1000, 10);
        if (uploadedFileResult.IsError)
        {
            LogMediaFileNotFound(logger, path);
            return Result.Err<StationMedia>($"Uploaded file did not become station media.");
        }

        var media = uploadedFileResult.Value.Media;
        media.ShouldNotBeNull();
        return Result.Ok(media);
    }

    private string GetFilePath(AzuraCastMediaDirectory directory)
    {
        var fileName = Guid.NewGuid() + ".mp3";
        return Path.Combine(GetLocationForDirectory(directory), fileName);
    }

    private string GetLocationForDirectory(AzuraCastMediaDirectory directory) =>
        directory switch
        {
            AzuraCastMediaDirectory.Archives => installation.Value.ArchiveSetLocation,
            AzuraCastMediaDirectory.Prerecords => installation.Value.PrerecordedSetLocation,
            _ => throw new NotImplementedException()
        };

    private async Task<Result<StationFileListItem, string>> GetUploadedFileAsync(
        string path,
        AzuraCastMediaDirectory directory)
    {
        var archiveListResult =
            await client.GetMediaInDirectoryAsync(GetLocationForDirectory(directory));

        if (archiveListResult.IsError)
            return Result.Err<StationFileListItem>("Failed to retrieve files from AzuraCast.");

        var uploadedFile = archiveListResult.Value
                                            .FirstOrDefault(file => file.PathShort == path.Split('/').Last());

        if (uploadedFile is null)
            return Result.Err<StationFileListItem>("Uploaded file not found in AzuraCast prerecorded directory.");

        return Result.Ok<StationFileListItem, string>(uploadedFile);
    }

    [LoggerMessage(LogLevel.Error, "Media file not found after upload: {path}")]
    static partial void LogMediaFileNotFound(ILogger<AzuraCastMediaUploader> logger, string path);
}