using System.Globalization;
using FastEndpoints;
using FFMpegCore;
using FluentValidation.Results;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Adapter.AzuraCast.Mappers;
using LowPressureZone.Api.Converters;
using LowPressureZone.Api.Endpoints.Schedules.HourlySlots;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Models;
using LowPressureZone.Api.Models.Configuration;
using LowPressureZone.Api.Services.Audio;
using LowPressureZone.Core;
using LowPressureZone.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shouldly;

namespace LowPressureZone.Api.Services.Files;

[RegisterService<PrerecordedMixFileProcessor>(LifeTime.Scoped)]
public sealed class PrerecordedMixFileProcessor(
    FormFileSaver fileSaver,
    MediaAnalyzer mediaAnalyzer,
    Mp3Processor mp3Processor,
    DataContext dataContext,
    IAzuraCastClient azuraCastClient,
    HourlySlotRequestToAzuraCastPlaylistConverter requestToPlaylistConverter,
    IOptions<AzuraCastInstallationConfiguration> installationOptions)
{
    private readonly string _prerecordedSetLocation = installationOptions.Value.PrerecordedSetLocation;
    private const int PrerecordedDurationMinutesTolerance = 2;

    public async Task<Result<string, IEnumerable<ValidationFailure>>> ProcessRequestFileToMp3Async(
        HourlySlotRequest request,
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
        var analysisValidationFailures = ValidateMediaAnalysis(request, analysis);
        if (analysisValidationFailures.Count != 0)
        {
            _ = await fileSaver.DeleteFileAsync(saveResult.Value);
            return Result.Err<string>(analysisValidationFailures);
        }

        var processResult = await ProcessToNewFile(analysis, saveResult.Value);
        _ = await fileSaver.DeleteFileAsync(saveResult.Value);

        if (processResult.IsError)
            return Result.Err<string>(processResult.Error);

        return Result.Ok<string, IEnumerable<ValidationFailure>>(processResult.Value);
    }
    
    public static ICollection<ValidationFailure> ValidateMediaAnalysis(HourlySlotRequest request, IMediaAnalysis analysis)
    {
        request.File.ShouldNotBeNull();
        List<ValidationFailure> failures = new();
        var timeslotDuration = request.StartsAt.AddHours(request.Duration) - request.StartsAt;
        if (TimeSpan.FromMinutes(timeslotDuration.TotalMinutes - PrerecordedDurationMinutesTolerance) >
            analysis.Duration
            || TimeSpan.FromMinutes(timeslotDuration.TotalMinutes + PrerecordedDurationMinutesTolerance) <
            analysis.Duration)
            failures.Add(new ValidationFailure(nameof(request.File),
                                               "Media file duration does not match the specified timeslot duration. Ensure it is +/- 2 minutes from the timeslot duration."));

        failures.AddRange(AudioQualityValidator.ValidateAudioQuality(analysis, request.File.Length,
                                                                     nameof(request.File)));
        return failures;
    }

    public async Task<Result<int, ValidationFailure>> EnqueuePrerecordedMixAsync(
        HourlySlotRequest request,
        DateTimeOffset scheduleStart,
        string localFilePath,
        CancellationToken ct = default)
    {
        var newMetadata = await GetAudioMetadataAsync(request, scheduleStart, ct);
        var fileName = GetUploadFileName(newMetadata.Artist, newMetadata.Title, request.StartsAt);

        var azuraCastFilePath = $"{_prerecordedSetLocation}/{fileName}";
        var uploadResult = await azuraCastClient.UploadMediaViaSftpAsync(localFilePath, azuraCastFilePath);
        if (uploadResult.IsError)
            return Result.Err<int>(uploadResult.Error.ToValidationFailure(nameof(request.File)));

        var uploadedFileResult = await Retry.RetryAsync(async () => await GetUploadedFileAsync(azuraCastFilePath),
                                                        result => result.IsError
                                                                  || (result.IsSuccess
                                                                      && result.Value.Media is not null),
                                                        1000,
                                                        10,
                                                        ct);
        if (uploadedFileResult.IsError)
            return Result.Err<int>(uploadedFileResult.Error.ToValidationFailure(nameof(request.File)));

        var uploadedFile = uploadedFileResult.Value;
        uploadedFile.Media.ShouldNotBeNull();

        var playlist = await requestToPlaylistConverter.ConvertAsync(request, ct);
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

    public async Task<Result<int, IEnumerable<ValidationFailure>>> UpdateEnqueuedPrerecordedMixAsync(
        Guid hourlySlotId,
        HourlySlotRequest request,
        CancellationToken ct = default)
    {
        // Get necessary data
        var hourlySlot = await dataContext.HourlySlots
                                          .Include(hourlySlot => hourlySlot.Schedule)
                                          .Where(hourlySlot => hourlySlot.Id == hourlySlotId)
                                          .FirstOrDefaultAsync(ct);

        hourlySlot.ShouldNotBeNull();
        hourlySlot.Prerecord.AzuraCastMediaId.ShouldNotBeNull();

        var getExistingMediaResult = await azuraCastClient.GetMediaAsync(hourlySlot.Prerecord.AzuraCastMediaId.Value);
        if (getExistingMediaResult.IsError)
            return Result.Err<int>("Unable to load existing media in AzuraCast".ToValidationFailures());

        var playlistId = getExistingMediaResult.Value.Playlists.FirstOrDefault()?.Id;
        if (playlistId is null)
            return Result.Err<int>("Unable to load playlist in AzuraCast".ToValidationFailures());

        // If there is a new file, we need to delete the old one and upload the new one
        StationMedia? newMedia = null;
        if (request.File is not null)
        {
            var processResult = await ProcessRequestFileToMp3Async(request, ct);
            if (processResult.IsError)
                return Result.Err<int>(processResult.Error);

            var localFilePath = processResult.Value;

            var deleteMediaResult = await azuraCastClient.DeleteMediaAsync(hourlySlot.Prerecord.AzuraCastMediaId.Value);
            if (deleteMediaResult.IsError)
                return Result.Err<int>((deleteMediaResult.Error.ReasonPhrase ??
                                        "Unable to delete existing media in AzuraCast")
                                       .ToValidationFailures(nameof(request.File)));

            var azuraCastFilePath = Path.Combine(_prerecordedSetLocation, Path.GetFileName(localFilePath));
            var uploadResult = await azuraCastClient.UploadMediaViaSftpAsync(localFilePath, azuraCastFilePath);
            if (uploadResult.IsError)
                return Result.Err<int>(uploadResult.Error.ToValidationFailures(nameof(request.File)));

            var uploadedFileResult = await Retry.RetryAsync(async () => await GetUploadedFileAsync(azuraCastFilePath),
                                                            result => result.IsError
                                                                      || (result.IsSuccess
                                                                          && result.Value.Media is not null),
                                                            1000,
                                                            10,
                                                            ct);
            if (uploadedFileResult.IsError)
                return Result.Err<int>(uploadedFileResult.Error.ToValidationFailures(nameof(request.File)));

            var uploadedFile = uploadedFileResult.Value;
            uploadedFile.Media.ShouldNotBeNull();
            newMedia = uploadedFile.Media;
        }

        // Update the metadata and playlist reference on the target media.
        var targetMedia = newMedia ?? getExistingMediaResult.Value;
        var updateMediaRequest = StationMediaMapper.ToRequest(targetMedia);
        var metadata = await GetAudioMetadataAsync(request, hourlySlot.Schedule.TimeRange.StartsAt, ct);
        updateMediaRequest.Title = metadata.Title;
        updateMediaRequest.Artist = metadata.Artist;
        updateMediaRequest.Playlists = [playlistId.Value];
        var updateMediaResult = await azuraCastClient.PutMediaAsync(targetMedia.Id, updateMediaRequest);
        if (updateMediaResult.IsError)
            return Result.Err<int>("Failed to update media metadata in AzuraCast".ToValidationFailures());

        // Both outcomes require a playlist update
        var playlistFromTimeslotResult = await requestToPlaylistConverter.ConvertAsync(request, ct);
        if (playlistFromTimeslotResult.IsError)
            return Result.Err<int>([playlistFromTimeslotResult.Error]);
        var timeslotPlaylist = playlistFromTimeslotResult.Value;
        timeslotPlaylist.Id = playlistId.Value;
        var putPlaylistResult = await azuraCastClient.PutPlaylistAsync(timeslotPlaylist);
        if (putPlaylistResult.IsError)
            return Result.Err<int>("Failed to update playlist in AzuraCast".ToValidationFailures());

        return Result.Ok<int, IEnumerable<ValidationFailure>>(targetMedia.Id);
    }

    private async Task<Result<string, IEnumerable<ValidationFailure>>> ProcessToNewFile(
        IMediaAnalysis analysis,
        string inputFilePath)
    {
        string outputFilePath;
        analysis.AudioStreams.ShouldHaveSingleItem();
        if (analysis.AudioStreams[0].CodecName == "mp3")
        {
            var stripResult = await mp3Processor.StripMp3MetadataAsync(inputFilePath);
            if (stripResult.IsError)
                return Result.Err<string>(stripResult.Error.ToValidationFailures(nameof(HourlySlotRequest.File)));

            outputFilePath = stripResult.Value;
        }
        else
        {
            var conversionResult = await mp3Processor.ConvertFileToMp3Async(inputFilePath);
            if (conversionResult.IsError)
                return Result.Err<string>(conversionResult.Error.ToValidationFailures(nameof(HourlySlotRequest.File)));

            outputFilePath = conversionResult.Value;
        }

        return Result.Ok<string, IEnumerable<ValidationFailure>>(outputFilePath);
    }

    private async Task<SimpleAudioMetadata> GetAudioMetadataAsync(
        HourlySlotRequest request,
        DateTimeOffset scheduleStart,
        CancellationToken ct)
    {
        var performerName = await dataContext.Performers
                                             .Where(performer => performer.Id == request.PerformerId)
                                             .Select(performer => performer.Name)
                                             .FirstAsync(ct);
        var title = string.IsNullOrWhiteSpace(request.Subtitle)
                        ? scheduleStart.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                        : request.Subtitle;
        return new SimpleAudioMetadata(title,
                                       performerName);
    }

    private static string GetUploadFileName(string artist, string title, DateTimeOffset start)
    {
        if (string.IsNullOrEmpty(title))
            return
                Path.GetFileName($"{artist} - {start.ToString("yyyy-MM-dd HH_mm", CultureInfo.InvariantCulture)}.mp3");

        return
            Path.GetFileName($"{artist} - {title} - {start.ToString("yyyy-MM-dd HH_mm", CultureInfo.InvariantCulture)}.mp3");
    }

    private async Task<Result<StationFileListItem, string>> GetUploadedFileAsync(string filePath)
    {
        var prerecordListResult = await azuraCastClient.GetMediaInDirectoryAsync(_prerecordedSetLocation);

        if (prerecordListResult.IsError)
            return Result.Err<StationFileListItem>("Failed to retrieve files from AzuraCast");

        var uploadedFile = prerecordListResult.Value
                                              .FirstOrDefault(file => file.PathShort == filePath.Split('/').Last());

        if (uploadedFile is null)
            return Result.Err<StationFileListItem>("Uploaded file not found in AzuraCast prerecorded directory");

        return Result.Ok<StationFileListItem, string>(uploadedFile);
    }
}