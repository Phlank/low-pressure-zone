using FastEndpoints;
using LowPressureZone.Core;

namespace LowPressureZone.Api.Services.Files;

[RegisterService<FormFileSaver>(LifeTime.Singleton)]
public sealed class FormFileSaver(
    EmailService emailer,
    ILogger<FormFileSaver> logger)
{
    private static string GetPathForFileName(string fileName) => Path.Combine(Path.GetTempPath(), fileName);

    public async Task<Result<string, string>> SaveFormFileAsync(IFormFile file, CancellationToken ct = default)
    {
        var path = GetPathForFileName(Guid.NewGuid().ToString());

        try
        {
            if (File.Exists(path)) File.Delete(path);

            await using var fileStream = File.Create(path);
            await file.CopyToAsync(fileStream, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to save uploaded file: {ErrorMessage}", ex.Message);
            return Result.Err<string, string>("Failed to save uploaded file.");
        }

        return Result.Ok(path);
    }

    public async Task<Result<string, string>> DeleteFileAsync(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                return Result.Ok(path);
            }

            logger.LogError("Failed to delete saved file at {Path} because it was not found.", path);
            return Result.Err<string>($"File does not exist at path: {path}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete saved file at {Path}: {ErrorMessage}", path, ex.Message);
            _ = await emailer.SendAdminMessage($"Failed to delete saved file at {path}: {ex.Message}",
                                               "Failed to delete saved file");
            return Result.Err<string, string>($"Failed to delete saved file at {path}: {ex.Message}");
        }
    }
}