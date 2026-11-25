using Aspire.Hosting.AWS;
using Aspire.Hosting.AWS.CloudFormation;

namespace LowPressureZone.Aspire.Extensions.LocalStack;

public class LocalStackResources
{
    public required IResourceBuilder<ILocalStackResource> LocalStack { get; init; }
    public required IResourceBuilder<ICloudFormationTemplateResource> AwsResources { get; init; }
    public required string Endpoint { get; init; }
    public required IResourceBuilder<ExecutableResource> S3Seed { get; init; }
}