using Amazon.S3;
using Amazon.S3.Model;
using LowPressureZone.Adapter.AzuraCast.Configuration;
using LowPressureZone.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Adapter.AzuraCast.Clients;

public class AzuraCastMediaClient(IAmazonS3 s3Client, IOptions<AzuraCastInstallationConfiguration> azuraCastInstallationConfiguration)
{
    public Task UploadToArchiveAsync(string name, Stream stream, CancellationToken ct)
    {
        var putRequest = new PutObjectRequest()
        {
            InputStream = stream,
            Key = $"{azuraCastInstallationConfiguration.Value.ArchiveSetLocation}/{name}"
        };
        return s3Client.PutObjectAsync(putRequest, ct);
    }
}