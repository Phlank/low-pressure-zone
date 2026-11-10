namespace LowPressureZone.Infrastructure.Configuration;

public sealed class AwsConfiguration
{
    public const string Name = "AwsConfiguration";
    
    public required string Region { get; init; }
    public required string Endpoint { get; init; }
    public required string AccessKey { get; init; }
    public required string SecretKey { get; init; }
}