using System.Globalization;
using FFMpegCore;
using FluentValidation.Results;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Adapter.AzuraCast.Mappers;
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
    IOptions<FileConfiguration> fileOptions)
{
    private readonly string _tempLocation = fileOptions.Value.TemporaryLocation;
    private readonly string _prerecordedSetLocation = fileOptions.Value.AzuraCastPrerecordedSetLocation;

    public async Task<Result<string, IEnumerable<ValidationFailure>>> ProcessUploadedMediaFileAsync(
        TimeslotRequest request,
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

        var newMetadata = await GetAudioMetadataAsync(request, ct);
        var fileName = GetUploadFileName(newMetadata.Artist, newMetadata.Title, request.StartsAt);
        var outputFilePath = Path.Combine(_tempLocation, fileName);

        var processResult = await ProcessToNewFile(analysis, saveResult.Value, outputFilePath);
        _ = await fileSaver.DeleteFileAsync(saveResult.Value);

        if (processResult.IsError)
            return Result.Err<string>(processResult.Error);

        var azuraCastFilePath = $"{_prerecordedSetLocation}/{fileName}";
        Result<string, string> uploadResult;
        await using (var fileStream = new FileStream(processResult.Value, FileMode.Open, FileAccess.Read))
        {
            uploadResult = await azuraCastClient.UploadMediaAsync(azuraCastFilePath, fileStream);
            _ = await fileSaver.DeleteFileAsync(processResult.Value);
        }
        
        if (uploadResult.IsError)
            return Result.Err<string>(uploadResult.Error.ToValidationFailures(nameof(request.File)));
        
        var uploadedFileResult = await GetUploadedFile(azuraCastFilePath);
        if (uploadedFileResult.IsError)
            return Result.Err<string>(uploadedFileResult.Error.ToValidationFailures(nameof(request.File)));

        var uploadedFile = uploadedFileResult.Value;

        var playlist = ConvertTimeslotToPlaylist(request, newMetadata);
        var createPlaylistResult = await azuraCastClient.PostPlaylistAsync(playlist);
        if (createPlaylistResult.IsError)
            return Result.Err<string>("Failed to create playlist in AzuraCast"
                                          .ToValidationFailures(nameof(request.File)));

        var playlistId = createPlaylistResult.Value;
        var updateRequest = StationMediaMapper.ToRequest(uploadedFile.Media!);
        updateRequest.Title = newMetadata.Title;
        updateRequest.Artist = newMetadata.Artist;
        updateRequest.Playlists = [playlistId];
        var updateMediaResult = await azuraCastClient.PutMediaAsync(uploadedFile.Media.Id, updateRequest);
        if (updateMediaResult.IsError)
            return Result.Err<string>("Failed to update media metadata in AzuraCast"
                                          .ToValidationFailures(nameof(request.File)));

        return Result.Ok<string, IEnumerable<ValidationFailure>>(azuraCastFilePath);
    }

    private async Task<Result<string, IEnumerable<ValidationFailure>>> ProcessToNewFile(
        IMediaAnalysis analysis,
        string inputFilePath,
        string outputFilePath)
    {
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

    private async Task<SimpleAudioMetadata> GetAudioMetadataAsync(TimeslotRequest request, CancellationToken ct)
    {
        var performerName = await dataContext.Performers
                                             .Where(performer => performer.Id == request.PerformerId)
                                             .Select(performer => performer.Name)
                                             .FirstAsync(ct);
        return new SimpleAudioMetadata(request.Name, performerName);
    }

    private static string GetUploadFileName(string artist, string? title, DateTimeOffset start)
    {
        if (string.IsNullOrEmpty(title))
            return $"{artist} - {start.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)}.mp3";

        return $"{artist} - {title} - {start.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)}.mp3";
    }

    private static StationPlaylist ConvertTimeslotToPlaylist(TimeslotRequest request, SimpleAudioMetadata metadata)
    {
        var playlistEnd = request.EndsAt.AddMinutes(5);
        return new()
        {
            IsEnabled = true,
            Name = $"Prerecorded Slot - {metadata.Artist} - {metadata.Title}",
            Type = StationPlaylistType.Default,
            Order = StationPlaylistOrder.Sequential,
            Source = StationPlaylistSource.Songs,
            Weight = 1,
            BackendOptions = [StationPlaylistBackendOption.Interrupt, StationPlaylistBackendOption.SingleTrack],
            ScheduleItems =
            [
                new StationPlaylistScheduleItem
                {
                    Days = [],
                    StartDate = DateOnly.FromDateTime(playlistEnd.UtcDateTime),
                    StartTime = request.StartsAt.Hour * 100 + request.StartsAt.Minute,
                    EndDate = DateOnly.FromDateTime(playlistEnd.UtcDateTime),
                    EndTime = request.EndsAt.Hour * 100 + request.EndsAt.Minute,
                    LoopOnce = true
                }
            ]
        };
    }

    private async Task<Result<StationFileListItem, string>> GetUploadedFile(string filePath)
    {
        while (true)
        {
            var prerecordListResult = await azuraCastClient.GetStationFilesInDirectoryAsync(_prerecordedSetLocation,
                                                                                            useInternal: true,
                                                                                            flushCache: true);
            if (prerecordListResult.IsError)
                return Result.Err<StationFileListItem>("Failed to retrieve files from AzuraCast");

            var uploadedFile = prerecordListResult.Value
                                                  .FirstOrDefault(file => file.PathShort == filePath.Split('/')
                                                                                                    .Last());

            if (uploadedFile is null)
                return Result.Err<StationFileListItem>("Uploaded file not found in AzuraCast prerecorded directory");

            if (uploadedFile.Media is null)
            {
                // File is still processing, should only take one or two cycles
                // Needed for updating the file and adding it to a new playlist
                await Task.Delay(1000);
                continue;
            }
            
            return Result.Ok(uploadedFile);
        }
    }
}