using Microsoft.Extensions.Configuration;

namespace LowPressureZone.Aspire.Extensions;

public static class ResourceBuilderExtensions
{
    public static IResourceBuilder<TResource> AddConfigurationToEnvironment<TResource>(this IResourceBuilder<TResource> builder, IConfigurationSection section)
        where TResource : IResourceWithEnvironment
    {
        var environmentVariables = section.Flatten();
        foreach (var environmentVariable in environmentVariables)
        {
            builder.WithEnvironment(environmentVariable.Key, environmentVariable.Value);
        }

        return builder;
    }
}