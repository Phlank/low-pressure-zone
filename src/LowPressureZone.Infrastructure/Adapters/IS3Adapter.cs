using Amazon.S3.Model;

namespace LowPressureZone.Infrastructure.Adapters;

public interface IS3Adapter
{
    Task<PutObjectResponse> PutObjectAsync(Stream stream, string path, CancellationToken ct);
    Task<DeleteObjectResponse> DeleteObjectAsync(string path, CancellationToken ct);
}