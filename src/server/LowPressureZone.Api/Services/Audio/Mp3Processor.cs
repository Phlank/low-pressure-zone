using FFMpegCore;
using FFMpegCore.Enums;
using LowPressureZone.Api.Models.Configuration;
using LowPressureZone.Core;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Api.Services.Audio;

public sealed partial class Mp3Processor(ILogger<Mp3Processor> logger, IOptions<FileConfiguration> fileConfig)
{
    private readonly string _temporaryLocation = fileConfig.Value.TemporaryLocation;
    public async Task<Result<string, string>> ConvertFileToMp3Async(string inputFilePath)
    {
        var outputFilePath = Path.Combine(_temporaryLocation, $"{Guid.NewGuid()}.mp3");
        try
        {
            var isConversionSuccessful =
                await FFMpegArguments.FromFileInput(inputFilePath,
                                                    false)
                                     .OutputToFile(outputFilePath,
                                                   overwrite: true,
                                                   options => options.WithAudioCodec(AudioCodec.LibMp3Lame)
                                                                     .WithAudioBitrate(320)
                                                                     .WithoutMetadata())
                                     .ProcessAsynchronously(throwOnError: true);

            if (isConversionSuccessful)
                return Result.Ok(outputFilePath);

            LogUnableToConvertFile(logger, inputFilePath);
            return Result.Err<string>("Failed to process audio file.");
        }
        catch (Exception ex)
        {
            LogUnableToConvertFileException(logger, inputFilePath, ex.Message);
            return Result.Err<string>("Failed to process audio file.");
        }
    }

    public async Task<Result<string, string>> StripMp3MetadataAsync(string inputFilePath)
    {
        var outputFilePath = Path.Combine(_temporaryLocation, $"{Guid.NewGuid()}.mp3");
        try
        {
            var isMetadataStripSuccessful =
                await FFMpegArguments.FromFileInput(inputFilePath, 
                                                    false)
                                     .OutputToFile(outputFilePath, overwrite: true,
                                                   options => options.WithCopyCodec()
                                                                     .WithoutMetadata())
                                     .ProcessAsynchronously(throwOnError: true);
            if (isMetadataStripSuccessful)
                return Result.Ok(outputFilePath);

            LogUnableToStripMetadata(logger, inputFilePath);
            return Result.Err<string>("Failed to process MP3 file.");
        }
        catch (Exception ex)
        {
            LogUnableToStripMetadataException(logger, inputFilePath, ex.Message);
            return Result.Err<string>("Failed to process MP3 file.");
        }
    }

    [LoggerMessage(LogLevel.Error, "Unable to convert file to MP3 at {inputFilePath}")]
    static partial void LogUnableToConvertFile(ILogger<Mp3Processor> logger, string inputFilePath);

    [LoggerMessage(LogLevel.Error, "Unable to convert file to MP3 at {inputFilePath}: {errorMessage}")]
    static partial void LogUnableToConvertFileException(
        ILogger<Mp3Processor> logger,
        string inputFilePath,
        string errorMessage);

    [LoggerMessage(LogLevel.Error, "Unable to strip metadata from MP3 file at {inputFilePath}")]
    static partial void LogUnableToStripMetadata(ILogger<Mp3Processor> logger, string inputFilePath);

    [LoggerMessage(LogLevel.Error, "Unable to strip metadata from MP3 file at {inputFilePath}: {errorMessage}")]
    static partial void LogUnableToStripMetadataException(
        ILogger<Mp3Processor> logger,
        string inputFilePath,
        string errorMessage);
}