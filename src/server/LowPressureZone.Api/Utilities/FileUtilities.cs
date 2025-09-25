namespace LowPressureZone.Api.Utilities;

public static class FileUtilities
{
    public static async Task<string> ToBase64EncodedString(this IFormFile file)
    {
        using var input = file.OpenReadStream();
        using var ms = new MemoryStream((int)Math.Min(input.Length, int.MaxValue));

        var buffer = new byte[81920]; // 80 KB
        int read;
        while ((read = await input.ReadAsync(buffer.AsMemory(0, buffer.Length))) > 0)
            await ms.WriteAsync(buffer.AsMemory(0, read));

        return Convert.ToBase64String(ms.ToArray());
    }
}
