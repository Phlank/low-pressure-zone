using Amazon.S3;
using Amazon.S3.Model;
using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Adapter.AzuraCast.Resolvers;
using LowPressureZone.Api.Utilities;
using LowPressureZone.Core;
using LowPressureZone.Infrastructure;
using LowPressureZone.Infrastructure.Adapters;
using LowPressureZone.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Shouldly;
using Results = LowPressureZone.Core.Results;

namespace LowPressureZone.Api.Services.AzuraCast;

[RegisterService<AzuraCastMediaUploader>(LifeTime.Singleton)]
public class AzuraCastMediaUploader(
    IAzuraCastClient client,
    IS3Adapter s3,
    IAzuraCastMediaPathResolver pathResolver,
    IOptions<AzuraCastInstallationConfiguration> installation,
    ILogger<AzuraCastMediaUploader> logger)
{
    public async Task<Result<StationMedia, string>> UploadAndGetMediaAsync(
        Stream stream,
        AzuraCastMediaDirectory directory,
        string fileName)
    {
        var uploadPath = pathResolver.ResolveFilePath(directory, fileName);
        var uploadResult = await s3.PutObjectAsync(stream, uploadPath, CancellationToken.None);
        if (uploadResult.HttpStatusCode != System.Net.HttpStatusCode.OK)
            return Results.Err<StationMedia>("Unable to upload media.");

        var uploadedFileResult = await Retry.RetryAsync(async () => await GetUploadedFileAsync(fileName, directory),
                                                        result => result.IsError
                                                                  || (result.IsSuccess &&
                                                                      result.Value.Media is not null),
                                                        1000,
                                                        300);
        if (uploadedFileResult.IsError)
        {
            logger.LogError("Media file not found after upload: {Path}", fileName);
            return Results.Err<StationMedia>($"Uploaded file did not become station media.");
        }

        var media = uploadedFileResult.Value.Media;
        media.ShouldNotBeNull();
        return Results.Ok(media);
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
            await client.ListMediaAsync(directory);

        if (archiveListResult.IsError)
            return Results.Err<StationFileListItem>("Failed to retrieve files from AzuraCast.");

        var uploadedFile = archiveListResult.Value.FirstOrDefault(file => file.PathShort == path.Split('/').Last());

        if (uploadedFile is null)
            return Results.Err<StationFileListItem>("Uploaded file not found in AzuraCast prerecorded directory.");

        return Results.Ok<StationFileListItem, string>(uploadedFile);
    }
}