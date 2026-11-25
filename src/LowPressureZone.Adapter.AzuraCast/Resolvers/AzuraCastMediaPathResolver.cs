using FastEndpoints;
using LowPressureZone.Api.Services.AzuraCast;
using LowPressureZone.Infrastructure;
using LowPressureZone.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Adapter.AzuraCast.Resolvers;

[RegisterService<IAzuraCastMediaPathResolver>(LifeTime.Singleton)]
public class AzuraCastMediaPathResolver(IOptions<AzuraCastInstallationConfiguration> installation) : IAzuraCastMediaPathResolver
{
    public string ResolveDirectory(AzuraCastMediaDirectory directory) =>
        directory switch
        {
            AzuraCastMediaDirectory.Archives => installation.Value.ArchiveSetLocation,
            AzuraCastMediaDirectory.Prerecords => installation.Value.PrerecordedSetLocation,
            _ => throw new ArgumentOutOfRangeException(nameof(directory), directory, null)
        };
    
    public string ResolveFilePath(AzuraCastMediaDirectory directory, string fileName) =>
        $"{ResolveDirectory(directory)}/{Paths.CleanFileName(fileName)}";
}