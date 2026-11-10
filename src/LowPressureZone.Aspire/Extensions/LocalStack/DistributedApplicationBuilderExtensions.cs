using Amazon;
using Aspire.Hosting.LocalStack.Container;
using LocalStack.Client.Options;
using Microsoft.Extensions.Configuration;

namespace LowPressureZone.Aspire.Extensions.LocalStack;

public static class DistributedApplicationBuilderExtensions
{
    private static LocalStackOptions LocalStackOptions(int edgePort) =>
        new(useLocalStack: true,
            sessionOptions: new SessionOptions(),
            configOptions: new ConfigOptions(localStackHost: "localhost",
                                             useSsl: false,
                                             useLegacyPorts: false,
                                             edgePort: edgePort));

    extension(IDistributedApplicationBuilder builder)
    {
        public LocalStackResources AddLpzLocalStack()
        {
            var awsConfig = builder.AddAWSSDKConfig()
                                   .WithProfile("default")
                                   .WithRegion(RegionEndpoint.USEast1);

            var localStackPort = builder.Configuration.GetValue<int>("AppHost:Ports:LocalStack");
            var localStack = builder.AddLocalStack(awsConfig: awsConfig,
                                                   localStackOptions: LocalStackOptions(edgePort: localStackPort),
                                                   configureContainer: cfg =>
                                                   {
                                                       cfg.ContainerImage = "localstack/localstack";
                                                       cfg.ContainerImageTag = "latest";
                                                       cfg.Lifetime = ContainerLifetime.Persistent;
                                                       cfg.LogLevel = LocalStackLogLevel.Info;
                                                       cfg.Port = localStackPort;
                                                   })!
                                    .WithEnvironment("LOCALSTACK_AUTH_TOKEN",
                                                     builder.Configuration["LocalStack:Token"]!);

            var awsServices = builder.AddAWSCloudFormationTemplate("aws-resources", "localstackCloudFormation.json")
                                     .WaitForStart(localStack);

            var s3Seed = builder.AddExecutable("localstack-s3-seed",
                                               "aws",
                                               builder.AppHostDirectory,
                                               "--endpoint-url",
                                               $"http://localhost:{localStackPort}",
                                               "--region",
                                               "us-east-1",
                                               "s3",
                                               "sync",
                                               "../../tools/aws/s3",
                                               "s3://lpz-archive",
                                               "--no-progress")
                                .WithEnvironment("AWS_ACCESS_KEY_ID", "000000000000")
                                .WithEnvironment("AWS_SECRET_ACCESS_KEY", "000000000000")
                                .WithEnvironment("AWS_EC2_METADATA_DISABLED", "true")
                                .WaitForCompletion(awsServices);

            return new()
            {
                AwsResources = awsServices,
                LocalStack = localStack
            };
        }
    }
}