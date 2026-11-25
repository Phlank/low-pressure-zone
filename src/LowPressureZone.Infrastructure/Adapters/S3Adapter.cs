using Amazon.S3;
using Amazon.S3.Model;
using FastEndpoints;
using LowPressureZone.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Infrastructure.Adapters;

[RegisterService<IS3Adapter>(LifeTime.Singleton)]
public class S3Adapter(IAmazonS3 s3, IOptions<AwsConfiguration> awsConfig) : IS3Adapter
{
    public Task<PutObjectResponse> PutObjectAsync(Stream stream, string path, CancellationToken ct) =>
        s3.PutObjectAsync(new PutObjectRequest
                          {
                              BucketName = awsConfig.Value.S3Bucket,
                              Key = path,
                          },
                          ct);
    
    public Task<DeleteObjectResponse> DeleteObjectAsync(string path, CancellationToken ct) =>
        s3.DeleteObjectAsync(new DeleteObjectRequest
                             {
                                 BucketName = awsConfig.Value.S3Bucket,
                                 Key = path,
                             },
                             ct);
}