using System.Globalization;
using FFMpegCore;
using FluentValidation.Results;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Adapter.AzuraCast.Mappers;
using LowPressureZone.Api.Converters;
using LowPressureZone.Api.Endpoints.Schedules.Timeslots;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Models;
using LowPressureZone.Api.Models.Configuration;
using LowPressureZone.Api.Services.Audio;
using LowPressureZone.Core;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shouldly;

namespace LowPressureZone.Api.Services.Files;

public sealed class TimeslotFileProcessor(
    FormFileSaver fileSaver,
    MediaAnalyzer mediaAnalyzer,
    Mp3Processor mp3Processor,
    DataContext dataContext,
    IAzuraCastClient azuraCastClient,
    TimeslotRequestToAzuraCastPlaylistConverter playlistConverter,
    IOptions<FileConfiguration> fileOptions)
{
    private readonly string _tempLocation = fileOptions.Value.TemporaryLocation;
    private readonly string _prerecordedSetLocation = fileOptions.Value.AzuraCastPrerecordedSetLocation;

    public async Task<Result<string, IEnumerable<ValidationFailure>>> ProcessUploadToMp3Async(
        TimeslotRequest request,
        DateTimeOffset scheduleStart,
        CancellationToken ct = default)
    {
        request.File.ShouldNotBeNull();
        var saveResult = await fileSaver.SaveFormFileAsync(request.File, ct);
        if (saveResult.IsError)
            return Result.Err<string>(saveResult.Error.ToValidationFailures(nameof(request.File)));

        var analysisResult = await mediaAnalyzer.AnalyzeAsync(saveResult.Value, ct);
        if (analysisResult.IsError)
        {
            _ = await fileSaver.DeleteFileAsync(saveResult.Value);
            return Result.Err<string>(analysisResult.Error.ToValidationFailures(nameof(request.File)));
        }

        var analysis = analysisResult.Value;
        var analysisValidationFailures = TimeslotRequestValidator.ValidateMediaAnalysis(request, analysis);
        if (analysisValidationFailures.Count != 0)
        {
            _ = await fileSaver.DeleteFileAsync(saveResult.Value);
            return Result.Err<string>(analysisValidationFailures);
        }

        var newMetadata = await GetAudioMetadataAsync(request, scheduleStart, ct);
        var fileName = GetUploadFileName(newMetadata.Artist, newMetadata.Title, request.StartsAt);
        var outputFilePath = Path.Combine(_tempLocation, fileName);

        var processResult = await ProcessToNewFile(analysis, saveResult.Value, outputFilePath);
        _ = await fileSaver.DeleteFileAsync(saveResult.Value);

        if (processResult.IsError)
            return Result.Err<string>(processResult.Error);

        return Result.Ok<string, IEnumerable<ValidationFailure>>(processResult.Value);
    }

    public async Task<Result<int, ValidationFailure>> EnqueuePrerecordedMixAsync(
        TimeslotRequest request,
        DateTimeOffset scheduleStart,
        string localFilePath,
        CancellationToken ct = default)
    {
        var newMetadata = await GetAudioMetadataAsync(request, scheduleStart, ct);
        var fileName = GetUploadFileName(newMetadata.Artist, newMetadata.Title, request.StartsAt);

        var azuraCastFilePath = $"{_prerecordedSetLocation}/{fileName}";
        Result<string, string> uploadResult;
        await using (var fileStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read))
        {
            uploadResult = await azuraCastClient.UploadMediaViaSftpAsync(fileStream, azuraCastFilePath);
            _ = await fileSaver.DeleteFileAsync(localFilePath);
        }

        if (uploadResult.IsError)
            return Result.Err<int>(uploadResult.Error.ToValidationFailure(nameof(request.File)));

        var uploadedFileResult = await Retry.RetryAsync(10,
                                                        1000,
                                                        result => result.IsError
                                                                  || (result.IsSuccess
                                                                      && result.Value.Media is not null),
                                                        async () => await GetUploadedFileAsync(azuraCastFilePath),
                                                        ct);
        if (uploadedFileResult.IsError)
            return Result.Err<int>(uploadedFileResult.Error.ToValidationFailure(nameof(request.File)));

        var uploadedFile = uploadedFileResult.Value;
        uploadedFile.Media.ShouldNotBeNull();

        var playlist = await playlistConverter.ConvertAsync(request, ct);
        if (playlist.IsError)
            return Result.Err<int>(playlist.Error);
        
        var createPlaylistResult = await azuraCastClient.PostPlaylistAsync(playlist.Value);
        if (createPlaylistResult.IsError)
            return Result.Err<int>("Failed to create playlist in AzuraCast".ToValidationFailure(nameof(request.File)));

        var playlistId = createPlaylistResult.Value;

        var updateRequest = StationMediaMapper.ToRequest(uploadedFile.Media);
        updateRequest.Title = newMetadata.Title;
        updateRequest.Artist = newMetadata.Artist;
        updateRequest.Playlists = [playlistId];
        var updateMediaResult = await azuraCastClient.PutMediaAsync(uploadedFile.Media.Id, updateRequest);
        if (updateMediaResult.IsError)
            return Result.Err<int>("Failed to update media metadata in AzuraCast"
                                       .ToValidationFailure(nameof(request.File)));

        return Result.Ok<int, ValidationFailure>(uploadedFile.Media.Id);
    }

    public async Task<Result<int, string>> ReplaceEnqueuedMixAsync(
        Guid timeslotId,
        TimeslotRequest request,
        string localFilePath,
        CancellationToken ct = default)
    {
        var timeslot = await dataContext.Timeslots
                                        .Include(timeslot => timeslot.Schedule)
                                        .Where(timeslot => timeslot.Id == timeslotId)
                                        .FirstOrDefaultAsync(ct);
        timeslot.ShouldNotBeNull();
        timeslot.AzuraCastMediaId.ShouldNotBeNull();

        var getMediaResult = await azuraCastClient.GetMediaAsync(timeslot.AzuraCastMediaId.Value);
        if (getMediaResult.IsError)
            return Result.Err<int>("Unable to get existing media");

        var playlistId = getMediaResult.Value.Media?.Playlists.FirstOrDefault()?.Id;
        if (playlistId is null)
            return Result.Err<int>("Unable to determine playlist for prerecorded timeslot file");

        var deleteMediaResult = await azuraCastClient.DeleteMediaAsync(timeslot.AzuraCastMediaId.Value);
        if (deleteMediaResult.IsError)
            return Result.Err<int>("Unable to delete existing prerecorded timeslot file");

        var azuraCastFilePath = $"{_prerecordedSetLocation}/{localFilePath.Split('/').Last()}";
        var uploadResult = await azuraCastClient.UploadMediaViaSftpAsync(localFilePath, azuraCastFilePath);
        if (uploadResult.IsError)
            return Result.Err<int>(uploadResult.Error);
        
        var uploadedFileResult = await Retry.RetryAsync(10,
                                                        1000,
                                                        result => result.IsError
                                                                  || (result.IsSuccess
                                                                      && result.Value.Media is not null),
                                                        async () => await GetUploadedFileAsync(azuraCastFilePath), 
                                                        ct);
        if (uploadedFileResult.IsError)
            return Result.Err<int>(uploadedFileResult.Error);
        var uploadedFile = uploadedFileResult.Value;
        uploadedFile.Media.ShouldNotBeNull();
        
        var updateRequest = StationMediaMapper.ToRequest(uploadedFile.Media);
        var metadata = await GetAudioMetadataAsync(request, request.StartsAt, CancellationToken.None);
        updateRequest.Title = metadata.Title;
        updateRequest.Artist = metadata.Artist;
        updateRequest.Playlists = [playlistId.Value];
        var updateMediaResult = await azuraCastClient.PutMediaAsync(uploadedFile.Media.Id, updateRequest);
        if (updateMediaResult.IsError)
            return Result.Err<int>("Failed to update media metadata in AzuraCast");
        
        return Result.Ok<int, string>(uploadedFile.Media.Id);
    }

    private async Task<Result<string, IEnumerable<ValidationFailure>>> ProcessToNewFile(
        IMediaAnalysis analysis,
        string inputFilePath,
        string outputFilePath)
    {
        analysis.AudioStreams.ShouldHaveSingleItem();
        if (analysis.AudioStreams[0].CodecName == "mp3")
        {
            var stripResult = await mp3Processor.StripMp3MetadataAsync(inputFilePath, outputFilePath);
            if (stripResult.IsError)
                return Result.Err<string>(stripResult.Error.ToValidationFailures(nameof(TimeslotRequest.File)));
        }
        else
        {
            var conversionResult = await mp3Processor.ConvertFileToMp3Async(inputFilePath, outputFilePath);
            if (conversionResult.IsError)
                return Result.Err<string>(conversionResult.Error.ToValidationFailures(nameof(TimeslotRequest.File)));
        }

        return Result.Ok<string, IEnumerable<ValidationFailure>>(outputFilePath);
    }

    private async Task<SimpleAudioMetadata> GetAudioMetadataAsync(
        TimeslotRequest request,
        DateTimeOffset scheduleStart,
        CancellationToken ct)
    {
        var performerName = await dataContext.Performers
                                             .Where(performer => performer.Id == request.PerformerId)
                                             .Select(performer => performer.Name)
                                             .FirstAsync(ct);
        var title = string.IsNullOrWhiteSpace(request.Name)
                        ? scheduleStart.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                        : request.Name;
        return new SimpleAudioMetadata(title,
                                       performerName);
    }

    private static string GetUploadFileName(string artist, string title, DateTimeOffset start)
    {
        if (string.IsNullOrEmpty(title))
            return $"{artist} - {start.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)}.mp3";

        return $"{artist} - {title} - {start.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)}.mp3";
    }

    private async Task<Result<StationFileListItem, string>> GetUploadedFileAsync(string filePath)
    {
        var prerecordListResult = await azuraCastClient.GetMediaInDirectoryAsync(_prerecordedSetLocation,
                                                                                 useInternalMode: true,
                                                                                 flushCache: true);

        if (prerecordListResult.IsError)
            return Result.Err<StationFileListItem>("Failed to retrieve files from AzuraCast");

        var uploadedFile = prerecordListResult.Value
                                              .FirstOrDefault(file => file.PathShort == filePath.Split('/').Last());

        if (uploadedFile is null)
            return Result.Err<StationFileListItem>("Uploaded file not found in AzuraCast prerecorded directory");

        return Result.Ok<StationFileListItem, string>(uploadedFile);
    }
}