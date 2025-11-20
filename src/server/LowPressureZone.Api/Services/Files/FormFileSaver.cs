using LowPressureZone.Api.Models.Configuration;
using LowPressureZone.Core;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Api.Services.Files;

public sealed partial class FormFileSaver(
    IOptions<FileConfiguration> fileConfig,
    EmailService emailer,
    ILogger<FormFileSaver> logger)
{
    private readonly string _temporaryLocation = fileConfig.Value.TemporaryLocation;

    private string GetPathForFileName(string fileName) => Path.Combine(_temporaryLocation, fileName);

    public async Task<Result<string, string>> SaveFormFileAsync(IFormFile file, CancellationToken ct = default)
    {
        var path = GetPathForFileName(Guid.NewGuid().ToString());

        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            await using var fileStream = File.Create(path);
            await file.CopyToAsync(fileStream, ct);
        }
        catch (Exception ex)
        {
            LogSaveFailure(logger, ex.Message);
            return Result.Err<string, string>("Failed to save uploaded file.");
        }

        return Result.Ok(path);
    }

    public async Task<Result<string, string>> DeleteSavedFormFileAsync(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                return Result.Ok(path);
            }

            LogNotFoundDeleteFailure(logger, path);
            return Result.Err<string>($"File does not exist at path: {path}");
        }
        catch (Exception ex)
        {
            LogExceptionDeleteFailure(logger, path, ex.Message);
            _ = await emailer.SendAdminMessage($"Failed to delete saved file at {path}: {ex.Message}",
                                               "Failed to delete saved file");
            return Result.Err<string, string>($"Failed to delete saved file at {path}: {ex.Message}");
        }
    }
    
    [LoggerMessage(LogLevel.Error, "Failed to save uploaded file: {errorMessage}")]
    static partial void LogSaveFailure(ILogger<FormFileSaver> logger, string errorMessage);

    [LoggerMessage(LogLevel.Error, "Failed to delete saved file at {path} because it was not found.")]
    static partial void LogNotFoundDeleteFailure(ILogger<FormFileSaver> logger, string path);

    [LoggerMessage(LogLevel.Error, "Failed to delete file saved at {path}: {errorMessage}")]
    static partial void LogExceptionDeleteFailure(ILogger<FormFileSaver> logger, string path, string errorMessage);
}