using LowPressureZone.Api.Services.AzuraCast;

namespace LowPressureZone.Adapter.AzuraCast.Resolvers;

public interface IAzuraCastMediaPathResolver
{
    string ResolveFilePath(AzuraCastMediaDirectory directory, string fileName);
    string ResolveDirectory(AzuraCastMediaDirectory directory);
}